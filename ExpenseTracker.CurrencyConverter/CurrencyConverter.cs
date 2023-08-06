using ExpenseTracker.CurrencyConverter.Config;
using ExpenseTracker.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExpenseTracker.CurrencyConverter
{
    public class CurrencyConverter
    {
        //private string _cachePath = Path.Combine(PathUtils.AppDataPath(), "_cache", "currency_conversion.json");
        private static List<ConversionData> _cachedConversionData = new List<ConversionData>();
        private string _cachePath = "";
        public CurrencyConverter(string cachePath)
        {
            _cachePath = cachePath;

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
                await client.GetStreamAsync($"https://free.currconv.com/api/v7/convert?q={conversionKey}&compact=ultra&apiKey={Keys.CURRENCY_CONVERTER_API_KEY}");
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
    }
}
