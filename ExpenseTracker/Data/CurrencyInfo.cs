
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
