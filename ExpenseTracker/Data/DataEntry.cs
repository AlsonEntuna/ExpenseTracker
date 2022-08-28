using System;
using ExpenseTracker.Wpf;

namespace ExpenseTracker.Data
{
    [Serializable]
    public class DataEntry : ViewModel
    {
        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private float _amount;
        public float Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        private string _category;
        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        private string _comments;
        public string Comments
        {
            get => _comments;
            set => SetProperty(ref _comments, value);
        }

        public DataEntry() { }
    }
}
