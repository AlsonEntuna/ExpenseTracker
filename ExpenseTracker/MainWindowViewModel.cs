using ExpenseTracker.Data;
using ExpenseTracker.View;
using System.Diagnostics;
using System.Windows.Input;
using WPFWrappers;
using WPFWrappers.Command;

namespace ExpenseTracker
{
    class MainWindowViewModel : ViewModel
    {
        private VariableExpense _currentVarExpense;
        public VariableExpense CurrentVarExpense
        {
            get => _currentVarExpense;
            set => SetProperty(ref _currentVarExpense, value);
        }
        public ICommand CreateVariableExpenseCommand => new RelayCommand(f => { CreateVariableExpense(); }, f => true);
        public MainWindowViewModel()
        {
            // Load the data...
            DataHandler.LoadAppConfiguration();
        }

        private void CreateVariableExpense()
        {
            CreateVariableExpenseWindow window = new CreateVariableExpenseWindow();
            if (window.ShowDialog() ?? true)
            {
                CurrentVarExpense = window.Expense;
            }
        }
    }
}
