using System;
using WPFWrappers;

namespace ExpenseTracker.Data
{
    [Serializable]
    public class DataCategory : ViewModel
    {
        private string _categoryName;
        public string CategoryName
        {
            get => _categoryName;
            set => SetProperty(ref _categoryName, value);
        }

        public DataCategory() { }
    }
}
