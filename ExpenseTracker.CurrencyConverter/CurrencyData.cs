using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.CurrencyConverter
{
    [Serializable]
    public class CurrencyData
    {
        public DataCurrency Currency { get; set; }
        public float Amount { get; set; }
        public CurrencyData() { }
        public CurrencyData(DataCurrency currency, float amount)
        {
            Currency = currency;
            Amount = amount;
        }

        public override bool Equals(object obj)
        {
            if (obj is CurrencyData other)
            {
                return string.Equals(Currency.Code, other.Currency.Code);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Currency.GetHashCode();
        }
    }
}
