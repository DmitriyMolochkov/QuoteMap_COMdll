using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace QuoteMap.Utils
{
    public static class StringUtils
    {
        public static string GenerateRandomString(int length)
        {
            var rBytes = new byte[length];
            using (var crypto = new RNGCryptoServiceProvider()) 
                crypto.GetBytes(rBytes);
            
            var base64 = Convert.ToBase64String(rBytes);

            var result = Regex.Replace(base64, "[^A-Za-z0-9]", "");
            
            return result.Substring(result.Length - length);
        }
    }
}