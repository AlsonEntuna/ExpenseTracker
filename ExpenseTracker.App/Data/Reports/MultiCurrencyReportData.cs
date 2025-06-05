using System;

namespace ExpenseTracker.Data.Reports
{
    [Serializable]
    public class MultiCurrencyReportData
    {
        public string CurrencyCode { get; set; }
        public string PaymentChannel { get; set; }
        public float Amount { get; set; }
        
        public MultiCurrencyReportData() { }
        public MultiCurrencyReportData(string currencyCode, string paymentChannel, float amout) 
        {
            CurrencyCode = currencyCode;
            PaymentChannel = paymentChannel;
            Amount = amout;
        }

        public override bool Equals(object obj)
        {
            if (obj is MultiCurrencyReportData other)
            {
                return string.Equals(CurrencyCode, other.CurrencyCode);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode()
                , CurrencyCode.GetHashCode());
        }
    }
}
