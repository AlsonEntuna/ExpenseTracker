using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using WPFWrappers;
using WPFWrappers.Command;

namespace ExpenseTracker
{
    class MainViewModel : ViewModel
    {
        public ICommand CreateVariableExpenseCommand => new RelayCommand(f => { CreateVariableExpense(); }, f => true);
        public MainViewModel() { }

        private void CreateVariableExpense()
        {

        }
    }
}
