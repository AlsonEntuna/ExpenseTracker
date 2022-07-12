using ExpenseTracker.Data;
using ExpenseTracker.Utils;
using ExpenseTracker.View;
using ExpenseTracker.ViewModels;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Windows.Input;
using WPFWrappers;

namespace ExpenseTracker
{
    class MainWindowViewModel : ViewModel
    {
        #region ViewModels
        private readonly VariableExpenseViewModel variableExpenseViewModel = new VariableExpenseViewModel();
        public VariableExpenseViewModel VariableExpenseViewModel => variableExpenseViewModel;
        #endregion

        #region Commands
        public ICommand CreateVariableExpenseCommand => new RelayCommand(CreateVariableExpense);
        public ICommand OpenVariableExpenseCommand => new RelayCommand(OpenVariableExpense);
        #endregion

        public MainWindowViewModel()
        {
            // Load the data...
            DataHandler.LoadAppConfiguration();
            InitializeData();
        }

        private void InitializeData()
        {
            if (!string.IsNullOrEmpty(DataHandler.Config.DataLocation))
            {
                VariableExpenseViewModel.CurrentDisplayedExpense = JsonUtils.Deserialize<VariableExpense>(DataHandler.Config.DataLocation);
                variableExpenseViewModel.UpdateEventListeners();
            }
        }

        private void CreateVariableExpense()
        {
            CreateVariableExpenseWindow window = new();
            if (window.ShowDialog() ?? true)
            {
                VariableExpenseViewModel.CurrentDisplayedExpense = window.Expense;
            }
        }

        private void OpenVariableExpense()
        {
            VariableExpenseViewModel.OpenVariableExpense();
        }

        internal void SaveData()
        {
            VariableExpenseViewModel.SaveCurrentExpenseData();
        }
    }
}
