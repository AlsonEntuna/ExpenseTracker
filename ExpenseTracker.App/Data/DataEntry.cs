using System;
using System.Text.Json.Serialization;
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
            set
            {
                SetProperty(ref _description, value);
                RecordLastUpdated();
            }
        }

        private float _amount;
        /// <summary>
        /// Amount reflected is already the converted amount.
        /// </summary>
        public float Amount
        {
            get => _amount;
            set
            {
                SetProperty(ref _amount, value);
                RecordLastUpdated();
            }
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

                RecordLastUpdated();
            }
        }

        /// <summary>
        /// Category is the PaymentChannel, we're just retaining the legacy name for the old data...
        /// </summary>
        private string _category;
        public string Category
        {
            get => _category;
            set
            {
                SetProperty(ref _category, value);
                RecordLastUpdated();
            }
        }

        private string _paymentChannel;
        public string PaymentChannel
        {
            get => _paymentChannel;
            set
            {
                SetProperty(ref _paymentChannel, value);
                RecordLastUpdated();
            }
        }

        private string _expenseCategory;
        public string ExpenseCategory
        {
            get => _expenseCategory;
            set
            {
                SetProperty(ref _expenseCategory, value);
                RecordLastUpdated();
            }
        }

        private string _comments;
        public string Comments
        {
            get => _comments;
            set
            {
                SetProperty(ref _comments, value);
                RecordLastUpdated();
            }
        }

        private CurrencyInfo _currency;
        public CurrencyInfo Currency
        {
            get => _currency;
            set
            {
                SetProperty(ref _currency, value);
                RecordLastUpdated();
            }
        }

        private string _entryDate;
        public string EntryDate
        {
            get => _entryDate;
            set => SetProperty(ref _entryDate, value);
        }

        private string _lastUpdated;
        public string LastUpdated
        {
            get => _lastUpdated;
            set => SetProperty(ref _lastUpdated, value);
        }

        [JsonIgnore]
        public string GetMetaData
        {
            get
            {
                return $"Created: {EntryDate}\nUpdated: {LastUpdated}";
            }
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

        private void RecordLastUpdated()
        {
            LastUpdated = DateTime.Now.ToString();
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
