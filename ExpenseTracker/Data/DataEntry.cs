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
        /// <summary>
        /// Amount reflected is already the converted amount.
        /// </summary>
        public float Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        private float _originalAmount;
        public float OriginalAmount
        {
            get => _originalAmount;
            set
            {
                if (AppInstance.Connection.MainCurrency != Currency)
                {
                    if (AppInstance.Connection.MainCurrency != null)
                    {
                        // TODO: not really good implementation...
                        CurrencyConverter converter = new CurrencyConverter();
                        // TODO: we need to catch the issue here...
                        if (Currency != null)
                            Amount = converter.Convert(Currency.Code, AppInstance.Connection.MainCurrency.Code, value);
                        else
                            Amount = 0;
                    }
                    SetProperty(ref _originalAmount, value);
                }
                else
                {
                    SetProperty(ref _originalAmount, 0);
                }
            }
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

        private DataCurrency _currency;
        public DataCurrency Currency
        {
            get => _currency;
            set => SetProperty(ref _currency, value);
        }

        public DataEntry() { }

        public void ConvertToMainCurrency(DataCurrency _mainCurrency) 
        {
            OriginalAmount = Amount;
            CurrencyConverter converter = new CurrencyConverter();
            Amount = converter.Convert(Currency.Code, _mainCurrency.Code, OriginalAmount);
        }
    }
}
