using ExpenseTracker.Data;
using System.Windows;

namespace ExpenseTracker.View.Dialogs
{
    /// <summary>
    /// Interaction logic for AddCurrencyDialog.xaml
    /// </summary>
    public partial class AddCurrencyDialog : Window
    {
        public AddCurrencyDialog()
        {
            InitializeComponent();
            Combo_Currency.ItemsSource = DataCurrency.GenerateCurrencyList();
        }
    }
}
