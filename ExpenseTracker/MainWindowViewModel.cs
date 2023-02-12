using ExpenseTracker.Data;
using ExpenseTracker.Utils;
using ExpenseTracker.View;
using ExpenseTracker.ViewModels;
using System;
using ExpenseTracker.Wpf;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;

namespace ExpenseTracker
{
    class MainWindowViewModel : ViewModel
    {
        #region ViewModels
        private readonly VariableExpenseViewModel variableExpenseViewModel = new VariableExpenseViewModel();
        public VariableExpenseViewModel VariableExpenseViewModel => variableExpenseViewModel;
        #endregion

        #region Commaands
        public ICommand ExportCategoriesCommand => new RelayCommand(ExportCategories);
        public ICommand ImportCategoriesCommand => new RelayCommand(ImportCategories);
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
                try
                {
                    VariableExpense deserializedData = JsonUtils.Deserialize<VariableExpense>(DataHandler.Config.DataLocation);
                    if (deserializedData == null)
                        return;
                    VariableExpenseViewModel.CurrentDisplayedExpense = deserializedData;
                    // Detects and migrates old legacy data...
                    variableExpenseViewModel.CurrentDisplayedExpense.DetectAndMigrateLegacyData();
                    variableExpenseViewModel.UpdateEventListeners();
                }
                catch(Exception e)
                {
                    DataHandler.Config.DataLocation = string.Empty;
                    // TODO: have a logging system here...
                }
            }
        }

        public void CreateVariableExpense()
        {
            CreateVariableExpenseWindow window = new();
            window.ShowDialog();
            if (window.DialogResult == true)
            {
                VariableExpenseViewModel.CurrentDisplayedExpense = window.Expense;
                VariableExpenseViewModel.IsNewExpense = true;
            }
        }

        public void OpenToolsPanel()
        {
            ExpenseTrackerTools toolsWindow = new ExpenseTrackerTools
            {
                DataContext = this
            };
            toolsWindow.ShowDialog();
        }

        // TODO: implement import and export of categories
        private void ImportCategories() 
        {

        }
        private void ExportCategories() 
        {
            
        }

        public void OpenVariableExpense()
        {
            VariableExpenseViewModel.OpenVariableExpense();
        }

        internal void SaveData()
        {
            VariableExpenseViewModel.SaveCurrentExpenseData();
        }
    }
}
