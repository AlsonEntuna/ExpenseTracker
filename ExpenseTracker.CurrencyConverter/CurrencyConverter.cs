using System.Text.Json;

using ExpenseTracker.Tools;
using ExpenseTracker.CurrencyConverter.Config;

namespace ExpenseTracker.CurrencyConverter
{
    public class CurrencyConverter
    {
        //private string _cachePath = Path.Combine(PathUtils.AppDataPath(), "_cache", "currency_conversion.json");
        private static List<ConversionData> _cachedConversionData = new List<ConversionData>();
        private string _cachedPath = "";
        public CurrencyConverter(string cachePath)
        {
            _cachedPath = cachePath;
            if (string.IsNullOrEmpty(cachePath))
                throw new ArgumentNullException(nameof(cachePath));

            if (!Directory.Exists(Path.GetDirectoryName(_cachedPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(_cachedPath));

            if (!File.Exists(_cachedPath))
                JsonUtils.SerializeArray(_cachedPath, _cachedConversionData);
            else
                _cachedConversionData = JsonUtils.DeserializeArray<List<ConversionData>>(_cachedPath);
        }

        public static async Task<float> GetCurrencyConversion(string fromCurrencyCode, string toCurrencyCode)
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
        private ConversionData GetConversionData(string conversionKey)
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
                var data = GetConversionData(conversionData.Key);
                if (data != null)
                    data.Value = conversionData.Value;
            }
            JsonUtils.SerializeArray(_cachedPath, _cachedConversionData);
        }
    }
}
