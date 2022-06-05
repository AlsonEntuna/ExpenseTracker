using ExpenseTracker.View;
using System.Diagnostics;
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
            CreateVariableExpenseWindow window = new CreateVariableExpenseWindow();
            if (window.ShowDialog() ?? true)
            {
                Debug.WriteLine("Done");
            }
        }
    }
}
