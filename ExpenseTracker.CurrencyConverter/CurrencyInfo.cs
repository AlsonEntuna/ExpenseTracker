using System.Globalization;

namespace ExpenseTracker.CurrencyConverter
{
    [Serializable]
    public class CurrencyInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }

        public CurrencyInfo(string code, string name, string symbol)
        {
            Code = code;
            Name = name;
            Symbol = symbol;
        }

        public override string ToString()
        {
            return $"{Name} ({Symbol})";
        }

        public override bool Equals(object obj)
        {
            if (obj is CurrencyInfo otherCurrency)
            {
                return string.Equals(Code, otherCurrency.Code);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode()
                , Code.GetHashCode());
        }

        private static List<CurrencyInfo>? _cachedCurrencies;

        /// <summary>
        /// Shared currency list, built once. Building it walks every culture, so avoid calling GenerateCurrencyList repeatedly.
        /// </summary>
        public static IReadOnlyList<CurrencyInfo> AllCurrencies => _cachedCurrencies ??= GenerateCurrencyList().ToList();

        public static IEnumerable<CurrencyInfo> GenerateCurrencyList()
        {
            List<CurrencyInfo> list = new List<CurrencyInfo>();
            CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Select(ci => ci.Name).Distinct()
                .Select(id => new RegionInfo(id))
                .GroupBy(r => r.ISOCurrencySymbol)
                .Select(g => g.First()).ToList()
                .ForEach(r => list.Add(new CurrencyInfo(r.ISOCurrencySymbol, r.CurrencyEnglishName, r.CurrencySymbol)));
            return list;
        }
    }
}
