using ExpenseTracker.Data;
using System.Windows;
using System.Windows.Input;
using ExpenseTracker.Wpf.Dialog;
using System.Text.RegularExpressions;

namespace ExpenseTracker.View
{
    /// <summary>
    /// Interaction logic for CreateVariableExpenseWindow.xaml
    /// </summary>
    public partial class CreateExpenseEntry : Window
    {
        public DataEntry Entry { get; set; }
        public CreateExpenseEntry()
        {
            InitializeComponent();
            // Initialize the value
            UpdateCategoryList();
        }

        private void UpdateCategoryList()
        {
            CmbBox_ExpenseCategory.ItemsSource = DataHandler.DataCategories.ExpenseCategories.ToArray();
            CmbBox_PaymentChannel.ItemsSource = DataHandler.DataCategories.PaymentChannels.ToArray();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TxtBox_Description.Text == string.Empty || TxtBox_Amount.Text == string.Empty)
            {
                MessageBox.Show("Please supply all necessary information", "Input Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            Entry = new DataEntry()
            {
                Description = TxtBox_Description.Text,
                Amount = float.Parse(TxtBox_Amount.Text, System.Globalization.NumberStyles.Float),
                PaymentChannel = CmbBox_PaymentChannel.SelectedItem as string,
                ExpenseCategory = CmbBox_ExpenseCategory.SelectedItem as string
            };

            DialogResult = true;
            Close();
        }

        private void BtnCreatePaymentChannel_Click(object sender, RoutedEventArgs e)
        {
            NameDialog dialog = new NameDialog("Enter new PaymentChannel:");
            if (dialog.ShowDialog() ?? true)
            {
                if (DataHandler.AddPaymentChannel(dialog.InputText))
                {
                    UpdateCategoryList();
                }
            }
        }

        private void BtnExpenseCreateCategory_Click(object sender, RoutedEventArgs e)
        {
            NameDialog dialog = new NameDialog("Enter new ExpenseCategory:");
            if (dialog.ShowDialog() ?? true)
            {
                if (DataHandler.AddExpenseCategory(dialog.InputText))
                {
                    UpdateCategoryList();
                }
            }
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

        private static readonly Regex _regex = new Regex("[^0-9.-]+");
        private static bool IsNumeric(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void TxtBox_Amount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumeric(e.Text);
        }
    }
}
