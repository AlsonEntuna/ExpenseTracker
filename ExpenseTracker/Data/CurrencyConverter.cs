using System;
using System.Collections.Generic;

using ExpenseTracker.Utils;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.IO;

namespace ExpenseTracker.Data
{
    public class CurrencyConverter
    {
        public CurrencyConverter() { }

        static async Task<float> GetCurrencyConversion(string from, string to)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();

            string conversionKey = $"{to}_{from}";
            await using Stream stream =
                await client.GetStreamAsync($"https://free.currconv.com/api/v7/convert?q={conversionKey}&compact=ultra&apiKey={ExpenseSys.Constants.API_KEY}");
            var conversionData = JsonSerializer.DeserializeAsync<Dictionary<string, float>>(stream);

            conversionData.Result.TryGetValue(conversionKey, out float val);
            return val;
        }

        public async void Convert(DataEntry entry, string toCurrency)
        {
            float conversionRate = await GetCurrencyConversion(entry.Currency.Code, toCurrency);
            entry.Amount = (float)Math.Round(entry.Amount / conversionRate, 2);
        }
    }
}
