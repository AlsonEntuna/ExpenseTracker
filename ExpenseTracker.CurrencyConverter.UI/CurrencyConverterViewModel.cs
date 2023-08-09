using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;

using ExpenseTracker.Wpf;

namespace ExpenseTracker.CurrencyConverter.UI
{
    public class CurrencyConverterViewModel : ViewModel
    {
        #region Properties
        private CurrencyInfo _fromCurrency;
        public CurrencyInfo FromCurrency
        {
            get => _fromCurrency;
            set
            {
                SetProperty(ref _fromCurrency, value);
                Convert();
            }
        }
        private CurrencyInfo _toCurrency;
        public CurrencyInfo ToCurrency
        {
            get => _toCurrency;
            set
            {
                SetProperty(ref _toCurrency, value);
                Convert();
            }
        }
        public List<CurrencyInfo> CurrencyInfos { get; set; }

        private float _inputValue;
        public float InputValue
        {
            get => _inputValue;
            set
            {
                SetProperty(ref _inputValue, value);
                Convert();
            }
        }

        private float _convertedValue;
        public float ConvertedValue
        {
            get => _convertedValue;
            set => SetProperty(ref _convertedValue, value);
        }
        #endregion
        #region Commands
        public ICommand SwapCurrenciesCommand => new RelayCommand(SwapCurrencies);
        #endregion

        private CurrencyConverter _currencyConverter;
        public CurrencyConverterViewModel() { }
        public CurrencyConverterViewModel(CurrencyConverter converter) 
        {
            CurrencyInfos = CurrencyInfo.GenerateCurrencyList().ToList();
            if (converter != null)
                _currencyConverter = converter;

            InputValue = 1;
        }

        private async void Convert()
        {
            if (FromCurrency == null || ToCurrency == null)
            {
                ConvertedValue = 0f;
                return;
            }

            string conversionKey = $"{FromCurrency.Code}_{ToCurrency.Code}";
            float conversionRate;
            try
            {
                conversionRate = await _currencyConverter.GetCurrencyConversion(FromCurrency.Code, ToCurrency.Code);
                _currencyConverter.SaveToCacheData(new ConversionData(conversionKey, conversionRate));
            }
            catch
            {
                var data = _currencyConverter.GetCachedConversionData(conversionKey);
                if (data != null)
                    conversionRate = data.Value;
                else
                    conversionRate = 1;
            }
            ConvertedValue = (float)Math.Round(InputValue / conversionRate, 2);
        }

        private void SwapCurrencies()
        {
            CurrencyInfo cached = FromCurrency;
            FromCurrency = ToCurrency;
            ToCurrency = cached;
        }
    }
}
