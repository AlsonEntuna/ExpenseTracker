using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                return string.Equals(CurrencyCode, other.CurrencyCode);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return CurrencyCode.GetHashCode();
        }
    }
}
