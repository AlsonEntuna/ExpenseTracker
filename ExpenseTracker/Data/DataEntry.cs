using ExpenseTracker.Wpf;
using System;

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

        /// <summary>
        /// Category is the PaymentChannel, we're just retaining the legacy name for the old data...
        /// </summary>
        private string _category;
        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        private string _paymentChannel;
        public string PaymentChannel
        {
            get => _paymentChannel;
            set => SetProperty(ref _paymentChannel, value);
        }

        private string _expenseCategory;
        public string ExpenseCategory
        {
            get => _expenseCategory;
            set => SetProperty(ref _expenseCategory, value);
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
