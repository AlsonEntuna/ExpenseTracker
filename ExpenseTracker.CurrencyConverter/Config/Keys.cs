namespace ExpenseTracker.CurrencyConverter.Config
{
    public static class Keys
    {
        internal static string CURRENCY_CONVERTER_API_KEY = "";
        public static void SetAPIKey(string key)
        {
            CURRENCY_CONVERTER_API_KEY = key;
        }

        internal static bool HasValidApiKey() { return !string.IsNullOrEmpty(CURRENCY_CONVERTER_API_KEY); }
    }
}