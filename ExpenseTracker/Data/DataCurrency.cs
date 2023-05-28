using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace ExpenseTracker.Data
{
    [Serializable]
    public class DataCurrency
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public ObservableCollection<CurrencyInfo> AlternativeCurrencies { get; set; }

        public DataCurrency(string code, string name, string symbol)
        {
            Code = code;
            Name = name;
            Symbol = symbol;
        }

        public override string ToString()
        {
            return $"{Name} - {Symbol}";
        }

        public static IEnumerable<DataCurrency> GenerateCurrencyList()
        {
            List<DataCurrency> list = new List<DataCurrency>();
            CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Select(ci => ci.Name).Distinct()
                .Select(id => new RegionInfo(id))
                .GroupBy(r => r.ISOCurrencySymbol)
                .Select(g => g.First()).ToList()
                .ForEach(r => list.Add(new DataCurrency(r.ISOCurrencySymbol, r.CurrencyEnglishName, r.CurrencySymbol)));
            return list;
        }


        public void AddCurrency()
        {
            // TODO: implement this...
        }
    }
}
