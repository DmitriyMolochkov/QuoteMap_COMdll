# QuoteMap COM library

Build:
```shell
dotnet build --configuration Release
cd C:\Windows\Microsoft.NET\Framework64\{your_last_version}
regasm.exe {path_to_dll}\QuoteMap.dll /tlb:QuoteMap.tlb /codebase
```
____
Usage example in `C#`
```C#
var quoteMap = new QuoteMap.QuoteMap();
await quoteMap.StartSessionAsync();
await quoteMap.AddQuoteAsync("BINANCE:BTCUSD");
quoteMap.QuoteUpdated += quote => Console.WriteLine($"{quote.Ticker} -> {quote.Price}");
```

Usage example in `Excel VBA`
```VBA
Public WithEvents QuoteMap As QuoteMap.QuoteMap


Private Sub Workbook_Open()
    Set QuoteMap = New QuoteMap.QuoteMap
    QuoteMap.StartSessionSync
    QuoteMap.AddQuoteSync ("BINANCE:BTCUSD")
End Sub


Private Sub QuoteMap_QuoteUpdated(ByVal quote As QuoteMap.QuoteModel)
    Cells(1, 1) = quote.Ticker + ": " + CStr(quote.price)
End Sub
```
