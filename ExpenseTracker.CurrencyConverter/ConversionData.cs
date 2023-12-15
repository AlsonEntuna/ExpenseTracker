namespace ExpenseTracker.CurrencyConverter
{
    public class ConversionData
    {
        public string Key;
        public float Value;
        public ConversionData() { }
        public ConversionData(string key, float value)
        {
            Key = key;
            Value = value;
        }

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
