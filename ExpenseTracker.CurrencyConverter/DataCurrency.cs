using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.CurrencyConverter
{
    [Serializable]
    public class DataCurrency
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

        public override string ToString()
        {
            return $"{Name} - {Symbol}";
        }

        public override bool Equals(object obj)
        {
            if (obj is DataCurrency otherCurrency)
                return string.Equals(Code, otherCurrency.Code);
            else return false;
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
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
    }
}
