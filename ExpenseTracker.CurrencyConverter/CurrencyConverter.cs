using System.Text.Json;

using ExpenseTracker.Tools;
using ExpenseTracker.CurrencyConverter.Config;

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
            if (string.IsNullOrEmpty(cachePath))
                throw new ArgumentNullException(nameof(cachePath));

            if (!Directory.Exists(Path.GetDirectoryName(_cachePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(_cachePath));

            if (!File.Exists(_cachePath))
                JsonUtils.SerializeArray(_cachePath, _cachedConversionData);
            else
                _cachedConversionData = JsonUtils.DeserializeArray<List<ConversionData>>(_cachePath);
        }

        public async Task<float> GetCurrencyConversion(string fromCurrencyCode, string toCurrencyCode)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();

            string conversionKey = $"{toCurrencyCode}_{fromCurrencyCode}";
            await using Stream stream =
                await client.GetStreamAsync($"https://free.currconv.com/api/v7/convert?q={conversionKey}&compact=ultra&apiKey={Keys.CURRENCY_CONVERTER_API_KEY}");
            
            var conversionData = JsonSerializer.DeserializeAsync<Dictionary<string, float>>(stream);

            conversionData.Result.TryGetValue(conversionKey, out float val);

            return val;
        }

        /// <summary>
        /// Returns the cached conversiondata based on a conversionKey
        /// Ex. EUR_USD, PHP_EUR
        /// </summary>
        /// <param name="conversionKey"></param>
        /// <returns></returns>
        public ConversionData GetCachedConversionData(string conversionKey)
        {
            ConversionData conversionData = null;
            if (_cachedConversionData == null || _cachedConversionData.Count == 0)
                return conversionData;

            foreach (ConversionData cachedData in _cachedConversionData)
            {
                if (string.Equals(cachedData.Key, conversionKey))
                    conversionData = cachedData;
            }
            return conversionData;
        }

        public void SaveToCacheData(ConversionData conversionData) 
        {
            if (!_cachedConversionData.Contains(conversionData))
                _cachedConversionData.Add(conversionData);
            else
            {
                var data = GetCachedConversionData(conversionData.Key);
                if (data != null)
                    data.Value = conversionData.Value;
            }
            JsonUtils.SerializeArray(_cachePath, _cachedConversionData);
        }
    }
}
