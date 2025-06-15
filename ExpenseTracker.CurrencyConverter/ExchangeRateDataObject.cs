namespace ExpenseTracker.CurrencyConverter
{
    /// <summary>
    /// <a href="https://www.exchangerate-api.com">Rates By Exchange Rate API</a>
    /// </summary>
    public class ExchangeRateDataObject
    {
        public string result { get; set; }
        public string provider { get; set; }
        public string documentation { get; set; }
        public string terms_of_use { get; set; }
        public long time_last_update_unix { get; set; }
        public string time_last_update_utc { get; set; }
        public long time_next_update_unix { get; set; }
        public string time_next_update_utc { get; set; }
        public long time_eol_unix { get; set; }
        public string base_code { get; set; }
        public Dictionary<string, double> rates { get; set; }
    }
}
