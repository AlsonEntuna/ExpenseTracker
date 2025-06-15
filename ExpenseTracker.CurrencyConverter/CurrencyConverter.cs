using System.Text.Json;

using ExpenseTracker.Tools;
using ExpenseTracker.CurrencyConverter.Config;

namespace ExpenseTracker.CurrencyConverter
{
    public class CurrencyConverter
    {
        private static List<ConversionData> _cachedConversionData = new List<ConversionData>();
        private readonly string _cachePath;
        public CurrencyConverter(string cachePath)
        {
            _cachePath = cachePath;
            if (string.IsNullOrEmpty(cachePath))
                throw new ArgumentNullException(nameof(cachePath));

            if (!Directory.Exists(Path.GetDirectoryName(_cachePath)))
                Directory.CreateDirectory(_cachePath);

            if (!File.Exists(_cachePath))
                JsonUtils.SerializeArray(_cachePath, _cachedConversionData);
            else
                _cachedConversionData = JsonUtils.DeserializeArray<List<ConversionData>>(_cachePath);
        }

        [Obsolete("Please remove and replace with the GetConversion() function")]
        public async Task<float> GetCurrencyConversion(string fromCurrencyCode, string toCurrencyCode)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();

            string conversionKey = $"{toCurrencyCode}_{fromCurrencyCode}";
            await using Stream stream =
                await client.GetStreamAsync($"https://free.currconv.com/api/v7/convert?q={conversionKey}&compact=ultra&apiKey={Keys.CURRENCY_CONVERTER_API_KEY}");

            ValueTask<Dictionary<string, float>?> conversionData = JsonSerializer.DeserializeAsync<Dictionary<string, float>>(stream);
            if (conversionData.Result != null)
            {
                conversionData.Result.TryGetValue(conversionKey, out float val);
                return val;
            }
            return 0.0f;
        }

        // TODO: iron out the new conversion API
        public async Task<float> GetConversion(string from, string to)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();

            HttpResponseMessage response = await client.GetAsync($"https://open.er-api.com/v6/latest/{from}");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            //await using Stream stream = await client.GetStreamAsync($"https://open.er-api.com/v6/latest/{from}");
            //ValueTask<Dictionary<string, float>?> conversionData = JsonSerializer.DeserializeAsync<Dictionary<string, float>>(stream);
            //if (conversionData.Result != null)
            //{
            //    //conversionData.Result.TryGetValue(conversionKey, out float val);
            //    //return val;
            //    return 0.0f;
            //}
            return 0.0f;
        }

        /// <summary>
        /// Returns the cached conversiondata based on a conversionKey
        /// Ex. EUR_USD, PHP_EUR
        /// </summary>
        /// <param name="conversionKey"></param>
        /// <returns></returns>
        public ConversionData? GetCachedConversionData(string conversionKey)
        {
            if (_cachedConversionData.Count == 0)
            {
                return null;
            }

            foreach (ConversionData cachedData in _cachedConversionData)
            {
                if (string.Equals(cachedData.Key, conversionKey))
                {
                    return cachedData;
                }
            }
            return null;
        }

        public void SaveToCacheData(ConversionData conversionData) 
        {
            if (!_cachedConversionData.Contains(conversionData))
                _cachedConversionData.Add(conversionData);
            else
            {
                var data = GetCachedConversionData(conversionData.Key);
                if (data != null)
                {
                    data.Value = conversionData.Value;
                }
            }
            JsonUtils.SerializeArray(_cachePath, _cachedConversionData);
        }
    }
}
