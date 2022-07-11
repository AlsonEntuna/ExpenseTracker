using ExpenseTracker.Data;
using ExpenseTracker.Utils;
using ExpenseTracker.View;
using ExpenseTracker.ViewModels;
using System.Windows.Input;
using WPFWrappers;
using WPFWrappers.Command;

namespace ExpenseTracker
{
    class MainWindowViewModel : ViewModel
    {
        #region ViewModels
        private readonly VariableExpenseViewModel variableExpenseViewModel = new VariableExpenseViewModel();
        public VariableExpenseViewModel VariableExpenseViewModel => variableExpenseViewModel;
        #endregion

        #region Commands
        public ICommand CreateVariableExpenseCommand => new RelayCommand(f => { CreateVariableExpense(); }, f => true);
        public ICommand OpenVariableExpenseCommand => new RelayCommand(f => { OpenVariableExpense(); }, f => true);
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
    }
}
