namespace ExpenseTracker.CurrencyConverter
{
    public class ConversionData
    {
        public string Key;
        public float Value;

        public ConversionData()
        {
            Key = string.Empty;
            Value = 0;
        }
        public ConversionData(string key, float value)
        {
            Key = key;
            Value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj is ConversionData other)
            {
                return string.Equals(Key, other.Key);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode()
                , Key.GetHashCode());
        }
    }
}
