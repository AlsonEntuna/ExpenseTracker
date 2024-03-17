using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

using ExpenseTracker.CurrencyConverter;
using ExpenseTracker.Data.Savings;

namespace ExpenseTracker.View.PiggyBank
{
    /// <summary>
    /// Interaction logic for AddSavingsWindow.xaml
    /// </summary>
    public partial class AddSavingsWindow : Window
    {
        public SavingsData NewSavingsData;
        public AddSavingsWindow()
        {
            InitializeComponent();
            Combo_Currency.ItemsSource = CurrencyInfo.GenerateCurrencyList();
        }

        private void Btn_AddSavings_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            NewSavingsData = new SavingsData(
                Txt_SavingsName.Text
                , Txt_Description.Text
                , float.Parse(Txt_Amount.Text)
                , Combo_Currency.SelectedItem as CurrencyInfo);
            Close();
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+");
        private static bool IsNumeric(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void Txt_Amount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumeric(e.Text);
        }
    }
}
