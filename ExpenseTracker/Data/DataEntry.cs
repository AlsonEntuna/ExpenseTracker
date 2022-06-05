using System;

namespace ExpenseTracker.Data
{
    [Serializable]
    public class DataEntry
    {
        public float Amount { get; set; }
        public DataCategory Category { get; set; }

        public DataEntry() { }
    }
}
