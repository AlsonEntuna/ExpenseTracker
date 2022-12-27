using ExpenseTracker.Data;
using System.Windows;
using System.Windows.Input;

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
            Combo_Currency.ItemsSource = DataCurrency.GenerateCurrencyList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Expense = new VariableExpense()
            {
                Name = Txtbox_Name.Text,
                Description = Txtbox_Description.Text,
                CycleEndDate = DPicker_ExpenseDate.SelectedDate.Value,
                DataCurrency = Combo_Currency.SelectedItem as DataCurrency
            };
            Close();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
