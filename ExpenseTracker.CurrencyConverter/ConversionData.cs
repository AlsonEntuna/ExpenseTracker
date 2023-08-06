using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.CurrencyConverter
{
    public class ConversionData
    {
        public string Key;
        public float Value;
        public ConversionData() { }
        public override bool Equals(object obj)
        {
            if (obj is ConversionData other)
                return string.Equals(Key, other.Key);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }
}
