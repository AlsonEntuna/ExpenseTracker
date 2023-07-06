using System;
using System.Collections.Generic;

using ExpenseTracker.Utils;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.IO;

namespace ExpenseTracker.Data
{
    public class ConversionData
    {
        public string Key;
        public float Value;
        public ConversionData() { }
        public override bool Equals(object obj)
        {
            if (obj is ConversionData other)
                return string.Equals(Key, other.Key);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }
    public class CurrencyConverter
    {
        private string _cachePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "_cache", "currency_conversion.json");
        private static List<ConversionData> _cachedConversionData = new List<ConversionData>();
        public CurrencyConverter() 
        {
            if (!Directory.Exists(Path.GetDirectoryName(_cachePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(_cachePath));

            if (!File.Exists(_cachePath))
                JsonUtils.SerializeArray(_cachePath, _cachedConversionData);
            else
                _cachedConversionData = JsonUtils.DeserializeArray<List<ConversionData>>(_cachePath);
        }

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

        private ConversionData GetConversionData(string key) 
        {
            foreach (var conversionData in _cachedConversionData)
            {
                if (string.Equals(conversionData.Key, key))
                {
                    return conversionData;
                }
            }
            return null;
        }

        public async void Convert(DataEntry entry, string toCurrency)
        {
            float conversionRate = 1;
            string conversionKey = $"{toCurrency}_{entry.Currency.Code}";
            try
            {
                conversionRate = await GetCurrencyConversion(entry.Currency.Code, toCurrency);

                // Add to the cache
                ConversionData cachedData = new ConversionData()
                {
                    Key = conversionKey,
                    Value = conversionRate
                };
                if (!_cachedConversionData.Contains(cachedData))
                    _cachedConversionData.Add(cachedData);
                else
                {
                    var data = GetConversionData(cachedData.Key);
                    if (data != null)
                        data.Value = cachedData.Value;
                }
                JsonUtils.SerializeArray(_cachePath, _cachedConversionData);
            }
            catch
            {
                var data = GetConversionData(conversionKey);
                if (data != null)
                    conversionRate = data.Value;
            }
            entry.Amount = (float)Math.Round(entry.Amount / conversionRate, 2);
        }
    }
}
