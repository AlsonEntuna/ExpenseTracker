using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ExpenseTracker.Data
{
    internal class DataCurrency
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }

        public DataCurrency(string code, string name, string symbol)
        {
            Code = code;
            Name = name;
            Symbol = symbol;
        }

        // TODO: Implement currency list per entry soon...
        public static IEnumerable<DataCurrency> GenerateCurrencyList()
        {
            List<DataCurrency> list = new List<DataCurrency>();
            CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Select(ci => ci.LCID).Distinct()
                .Select(id => new RegionInfo(id))
                .GroupBy(r => r.ISOCurrencySymbol)
                .Select(g => g.First()).ToList()
                .ForEach(r => list.Add(new DataCurrency(r.ISOCurrencySymbol, r.CurrencyEnglishName, r.CurrencySymbol)));
            return list;
        }
    }
}
