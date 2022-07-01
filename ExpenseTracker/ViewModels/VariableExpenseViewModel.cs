using ExpenseTracker.Data;
using WPFWrappers;

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
        public VariableExpenseViewModel()
        {

        }
    }
}
