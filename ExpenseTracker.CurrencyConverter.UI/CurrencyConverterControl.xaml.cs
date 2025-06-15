using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ExpenseTracker.CurrencyConverter.UI
{
    /// <summary>
    /// Interaction logic for CurrencyConverterControl.xaml
    /// </summary>
    public partial class CurrencyConverterControl : UserControl
    {
        public CurrencyConverterControl()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
