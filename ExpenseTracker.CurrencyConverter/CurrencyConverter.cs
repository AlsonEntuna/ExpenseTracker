using ExpenseTracker.Tools;
using Newtonsoft.Json;

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

        // TODO: iron out the new conversion API
        public async Task<float> GetCurrencyConversion(string from, string to)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();

            HttpResponseMessage response = await client.GetAsync($"https://open.er-api.com/v6/latest/{from}");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            if (responseBody != string.Empty)
            {
                ExchangeRateDataObject dataobject = JsonConvert.DeserializeObject<ExchangeRateDataObject>(responseBody);
                if (dataobject != null && dataobject.rates.ContainsKey(to))
                {
                    return (float)dataobject.rates[to];
                }
            }
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
            {
                _cachedConversionData.Add(conversionData);
            }
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
