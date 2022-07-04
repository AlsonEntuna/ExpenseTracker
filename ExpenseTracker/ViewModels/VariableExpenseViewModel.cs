using ExpenseTracker.Data;
using ExpenseTracker.View;
using System.Windows.Input;
using WPFWrappers;
using WPFWrappers.Command;

namespace ExpenseTracker.ViewModels
{
    class VariableExpenseViewModel : ViewModel
    {
        private VariableExpense _expense;
        public VariableExpense Expense
        {
            get => _expense;
            set => SetProperty(ref _expense, value);
        }

        #region Commands
        public ICommand AddEntryCommand => new RelayCommand(f => { AddEntry(); }, f => true);
        public ICommand GenerateExpenseDataCommand => new RelayCommand(f => { GenerateExpenseData(); }, f => true);
        #endregion

        public VariableExpenseViewModel()
        {

        }

        private void AddEntry()
        {
            CreateExpenseEntry entryWindow = new CreateExpenseEntry();
            if (entryWindow.ShowDialog() ?? true)
            {
                Expense.Entries.Add(entryWindow.Entry);
            }
        }

        private void GenerateExpenseData()
        {

        }
    }
}
