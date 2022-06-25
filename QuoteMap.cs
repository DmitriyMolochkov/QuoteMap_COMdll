using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using QuoteMap.Utils;
using QuoteMap.TradingViewConnection;
using QuoteMap.TradingViewConnection.Messages;

namespace QuoteMap
{
    [ComVisible(false)]
    public delegate void QuoteEventHandler(QuoteModel quote);
    
    [ComVisible(true)]
    [Guid("a2fe6743-8c6e-4f94-946d-c4f22c96ae52")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComSourceInterfaces(typeof(IQuoteMapEvents))]
    public class QuoteMap : IQuoteMap, IDisposable
    {
        public event QuoteEventHandler QuoteUpdated;
        
        private readonly Connection _connection = new Connection();
        private readonly string _sessionId = $"qs_{StringUtils.GenerateRandomString(12)}";
        private bool _isOpenSession;
        private readonly Dictionary<string, QuoteModel> _quoteDictionary = 
            new Dictionary<string, QuoteModel>(StringComparer.OrdinalIgnoreCase);
        private readonly object _dictionaryLocker = new object(); 
        private bool _disposed;

        public QuoteMap()
        {
            #pragma warning disable 4014
            _connection.MessageReceived += (sender, args) => HandleMessage(args.Message);
            #pragma warning restore 4014
        }
        
        public async Task StartSessionAsync()
        {
            await _connection.ConnectionAsync();
            await SendMessageAsync(TradingViewMsgType.QuoteCreateSession);
            await SendMessageAsync(TradingViewMsgType.QuoteSetFields, new object[] { "lp" });
            _isOpenSession = true;
        }

        public void StartSessionSync() => StartSessionAsync().Wait();

        public QuoteModel GetQuote(string symbol)
        {
            lock (_dictionaryLocker)
            {
                if (_quoteDictionary.TryGetValue(symbol, out QuoteModel quoteModel))
                    return quoteModel;
            }

            return null;
        }

        public async Task<bool> AddQuoteAsync(string symbol)
        {
            CheckSession();
            
            lock (_dictionaryLocker)
            {
                if (_quoteDictionary.ContainsKey(symbol)) return false;
                
                _quoteDictionary.Add(symbol, new QuoteModel(symbol, Double.NaN));
            }
            
            await SendMessageAsync(TradingViewMsgType.QuoteAddSymbols, new object[] { symbol });
            
            return true;
        }

        public bool AddQuoteSync(string symbol) => AddQuoteAsync(symbol).GetAwaiter().GetResult();

        public async Task<bool> DeleteQuoteAsync(string symbol)
        {
            CheckSession();

            lock (_dictionaryLocker)
            {
                if (!_quoteDictionary.Remove(symbol)) return false;
            }
            
            await SendMessageAsync(TradingViewMsgType.QuoteRemoveSymbols, new object[] { symbol });
            return true;
        }

        public bool DeleteQuoteSync(string symbol) => DeleteQuoteAsync(symbol).GetAwaiter().GetResult();

        public async Task ClearQuotesAsync()
        {
            CheckSession();

            string[] symbols;
            lock (_dictionaryLocker)
            {
                symbols = _quoteDictionary.Keys.ToArray();
                _quoteDictionary.Clear();
            }
            
            await SendMessageAsync(TradingViewMsgType.QuoteRemoveSymbols, symbols.Cast<object>().ToArray());
        }

        public void ClearQuotesSync() => ClearQuotesAsync().Wait();

        private Task SendMessageAsync(TradingViewMsgType type) => SendMessageAsync(type, new object[] {});

        private async Task SendMessageAsync(TradingViewMsgType type, object[] parameters)
        {
            var data = new object[parameters.Length + 1];
            data[0] = _sessionId;
            parameters.CopyTo(data, 1);
            
            await _connection.SendAsync(new TradingViewMessage(type, data));
        }

        private async Task HandleMessage(TradingViewMessage message)
        {
            if (message.Parameters[0].ToString() != _sessionId) return;

            switch (message.Type)
            {
                case TradingViewMsgType.QuoteUpdated:
                {
                    string json = message.Parameters[1].ToString();
                    var updateMessage = JsonConvert.DeserializeObject<QuoteUpdateMessage>(json);
                    var quoteModel = new QuoteModel(updateMessage);
                    
                    bool isExistSymbol;
                    lock (_quoteDictionary)
                    {
                        isExistSymbol = _quoteDictionary.ContainsKey(quoteModel.Symbol);
                        if (isExistSymbol)
                        {
                            _quoteDictionary[quoteModel.Symbol] = quoteModel;
                            QuoteUpdated?.Invoke(quoteModel);
                        }
                    }
                    
                    if (!isExistSymbol) 
                        await SendMessageAsync(
                            TradingViewMsgType.QuoteRemoveSymbols, 
                            new object[] { quoteModel.Symbol });

                    return;
                }
                default: return;
            }
        }

        private void CheckSession()
        {
            if (!_isOpenSession) throw new SessionNotOpenException();
        }
        
        ~QuoteMap() => Dispose(false);
        
        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing) _connection.Dispose();
            _disposed = true;
        }
    }
}