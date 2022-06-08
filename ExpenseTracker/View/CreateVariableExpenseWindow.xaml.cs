using ExpenseTracker.Data;
using System.Windows;

namespace ExpenseTracker.View
{
    /// <summary>
    /// Interaction logic for CreateVariableExpenseWindow.xaml
    /// </summary>
    public partial class CreateVariableExpenseWindow : Window
    {
        public VariableExpense Expense;
        public CreateVariableExpenseWindow()
        {
            InitializeComponent();
            DPicker_ExpenseDate.SelectedDate = System.DateTime.Now;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Expense = new VariableExpense()
            {
                Name = Txtbox_Name.Text,
                Description = Txtbox_Description.Text,
                CycleEndDate = DPicker_ExpenseDate.SelectedDate.Value
            };
            Close();
        }
    }
}
