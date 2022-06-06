using System;
using WPFWrappers;

namespace ExpenseTracker.Data
{
    [Serializable]
    public class DataEntry : ViewModel
    {
        private float _amount;
        public float Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        private DataCategory _category;
        public DataCategory Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        public DataEntry() { }
    }
}
