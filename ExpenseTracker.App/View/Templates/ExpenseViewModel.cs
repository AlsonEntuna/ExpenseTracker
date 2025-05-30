using System.Collections.Generic;
using ExpenseTracker.Data;
using ExpenseTracker.Wpf;

namespace ExpenseTracker.View.Templates
{
    public class ExpenseViewModel : ViewModel
    {
        private VariableExpense _currentDisplayedExpense;
        public VariableExpense CurrentDisplayedExpense
        {
            get => _currentDisplayedExpense;
            set
            {
                SetProperty(ref _currentDisplayedExpense, value);
                // Set the Main Currency
                AppInstance.Connection.MainCurrency = CurrentDisplayedExpense.DataCurrency;
            }
        }

        private List<DataEntry> _selectedDataEntries;
        public List<DataEntry> SelectedDataEntries
        {
            get => _selectedDataEntries;
            set => SetProperty(ref _selectedDataEntries, value);
        }

        public List<string> Categories => DataHandler.DataCategories.ExpenseCategories;
        public List<string> PaymentChannels => DataHandler.DataCategories.PaymentChannels;

        public ExpenseViewModel()
        {

        }

    }
}