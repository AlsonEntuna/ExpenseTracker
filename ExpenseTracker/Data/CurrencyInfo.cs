
using System;
using System.Globalization;

namespace ExpenseTracker.Data
{
    [Serializable]
    public class CurrencyInfo
    {
        public CultureInfo MainCulture;
        public CultureInfo Culture;
        public CurrencyInfo(CultureInfo main, CultureInfo culture)
        {
            MainCulture = main;
            Culture = culture;
        }

        public void GenerateCurrencyValue()
        {

        }
    }
}
