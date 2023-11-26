using System.Windows;
using ExpenseTracker.ViewModels;

namespace ExpenseTracker.View
{
    /// <summary>
    /// Interaction logic for PiggyBankWindow.xaml
    /// </summary>
    public partial class PiggyBankWindow : Window
    {
        private PiggyBankViewModel _vm;
        public PiggyBankWindow()
        {
            InitializeComponent();
        }

        private void EnsureDataContext()
        {
            if (_vm != null) return;
            _vm = DataContext as PiggyBankViewModel;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            EnsureDataContext();
            _vm.Dispose();
        }
    }
}
