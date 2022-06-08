using ExpenseTracker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
