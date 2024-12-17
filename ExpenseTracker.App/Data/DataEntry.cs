using System;
using ExpenseTracker.CurrencyConverter;
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
                    
                    if (Currency != null)
                    {
                        Amount = value;
                        ConvertToMainCurrency();
                    }
                    SetProperty(ref _originalAmount, value);
                }
                else
                {
                    if (AppInstance.Connection.MainCurrency != null)
                        SetProperty(ref _originalAmount, 0);
                    else
                        SetProperty(ref _originalAmount, value);
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

        private CurrencyInfo _currency;
        public CurrencyInfo Currency
        {
            get => _currency;
            set => SetProperty(ref _currency, value);
        }

        public DataEntry() { }

        public override bool Equals(object obj)
        {
            if (obj is DataEntry otherEntry)
                return string.Equals(Description, otherEntry.Description)
                    && string.Equals(PaymentChannel, otherEntry.PaymentChannel)
                    && string.Equals(Category, otherEntry.Category)
                    && string.Equals(Currency.Code, otherEntry.Currency.Code);
            else 
                return false;
        }

        public async void ConvertToMainCurrency()
        {
            var fromCurrency = AppInstance.Connection.MainCurrency.Code;
            var conversionKey = $"{fromCurrency}_{Currency.Code}";
            float conversionRate;
            try
            {
                conversionRate = await AppInstance.Connection.CurrConverter.GetCurrencyConversion(Currency.Code, fromCurrency);
                AppInstance.Connection.CurrConverter.SaveToCacheData(new CurrencyConverter.ConversionData(conversionKey, conversionRate));
            }
            catch
            {
                var data = AppInstance.Connection.CurrConverter.GetCachedConversionData(conversionKey);
                conversionRate = data?.Value ?? 1.0f;
            }
            Amount = (float)Math.Round(Amount * conversionRate, 2);
        }

        public override int GetHashCode()
        {
            return Description.GetHashCode();
        }
    }
}
