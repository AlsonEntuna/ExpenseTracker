using System.Collections.Generic;
using System.IO;

using ExpenseTracker.CurrencyConverter;
using ExpenseTracker.Environment;
using ExpenseTracker.Tools;
using ExpenseTracker.Wpf;

namespace ExpenseTracker.Data
{
    internal class AppInstance
    {
        #region Singleton
        private static AppInstance _instance;
        public static AppInstance Connection
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppInstance();
                }
                return _instance;
            }
        }
        #endregion
       
        public CurrencyInfo MainCurrency;
        private List<ViewModel> _appViewModels = new List<ViewModel>();
        public CurrencyConverter.CurrencyConverter CurrConverter;

        private AppInstance() 
        {
            CurrConverter = new CurrencyConverter.CurrencyConverter(Path.Combine(PathUtils.AppDataPath(
                new string[] { Constants.EXPENSETRACKER, "_cache" }), 
                "currency_conversion.json"));
        }

        public void AddViewModel(ViewModel vm)
        {
            if (!_appViewModels.Contains(vm))
            {
                _appViewModels.Add(vm);
            }
        }

        public T GetEditorViewModel<T>() where T : ViewModel
        {
            foreach (ViewModel vm in _appViewModels)
            {
                if (vm is T)
                    return (T)vm;
            }
            return null;
        }
    }
}
