using ExpenseTracker.CurrencyConverter;

using System;

namespace ExpenseTracker.Data.Reports
{
    [Serializable]
    public class ReportData
    {
        public CurrencyInfo Currency { get; set; }
        public float Amount { get; set; }
        public ReportData() { }
        public ReportData(CurrencyInfo currency, float amount)
        {
            Currency = currency;
            Amount = amount;
        }

        public override bool Equals(object obj)
        {
            if (obj is ReportData other)
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
