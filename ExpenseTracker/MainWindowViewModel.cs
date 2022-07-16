using ExpenseTracker.Data;
using ExpenseTracker.Utils;
using ExpenseTracker.View;
using ExpenseTracker.ViewModels;
using WPFWrappers;

namespace ExpenseTracker
{
    class MainWindowViewModel : ViewModel
    {
        #region ViewModels
        private readonly VariableExpenseViewModel variableExpenseViewModel = new VariableExpenseViewModel();
        public VariableExpenseViewModel VariableExpenseViewModel => variableExpenseViewModel;
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
