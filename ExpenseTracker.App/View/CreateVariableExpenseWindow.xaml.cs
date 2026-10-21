using ExpenseTracker.CurrencyConverter;
using ExpenseTracker.Data;
using ExpenseTracker.Wpf.Controls;

using System.Windows;

namespace ExpenseTracker.View
{
    /// <summary>
    /// Interaction logic for CreateVariableExpenseWindow.xaml
    /// </summary>
    public partial class CreateVariableExpenseWindow : ExWindow
    {
        public VariableExpense Expense;
        public string ExpensePath;
        public CreateVariableExpenseWindow()
        {
            InitializeComponent();
            DPicker_ExpenseDate.SelectedDate = System.DateTime.Now;
            Combo_Currency.ItemsSource = CurrencyInfo.GenerateCurrencyList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Expense = new VariableExpense()
            {
                Name = Txtbox_Name.Text,
                Description = Txtbox_Description.Text,
                CycleEndDate = DPicker_ExpenseDate.SelectedDate.Value,
                DataCurrency = Combo_Currency.SelectedItem as CurrencyInfo
            };
            Close();
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
