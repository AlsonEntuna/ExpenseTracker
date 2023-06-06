using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using ExpenseTracker.Utils;


namespace ExpenseTracker.Data
{
    public class CurrencyConverter
    {
        private readonly Dictionary<string, object> _currencies;
        public CurrencyConverter() 
        {
            _currencies = JsonUtils.Deserialize<Dictionary<string,object>>("Resources/conversion/conversion_data_eur.json");
        }

        public float Convert(string fromCurrency, string toCurrency, float amount)
        {
            _currencies.TryGetValue("data", out object dataObj);
            JToken conversionToken = JsonUtils.GetJArrayValue((JObject)dataObj, fromCurrency);
            float conversionRate = (float)conversionToken["value"];
            float convertedAmount = (float)Math.Round(amount / conversionRate, 2);
            
            return convertedAmount;
        }
    }
}
