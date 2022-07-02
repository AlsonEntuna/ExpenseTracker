using ExpenseTracker.Data;
using System.Windows;

namespace ExpenseTracker.View
{
    /// <summary>
    /// Interaction logic for CreateVariableExpenseWindow.xaml
    /// </summary>
    public partial class CreateExpenseEntry : Window
    {
        public VariableExpense Expense { get; set; }
        public CreateExpenseEntry()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // TODO: build the data here....
            this.DialogResult = true;
            Close();
        }

        private void BtnCreateCategory_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
