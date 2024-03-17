using System.Linq;
using System.Windows;
using ExpenseTracker.Data.Savings;
using ExpenseTracker.ViewModels;

namespace ExpenseTracker.View.PiggyBank
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

        private void DataGrid_SavingsInput_CurrentCellChanged(object sender, System.EventArgs e)
        {
            EnsureDataContext();
            _vm?.ForceComputeSavingsData();
        }

        private void DataGrid_SavingsInput_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            EnsureDataContext();

            _vm.SelectedSavingsInput = DataGrid_SavingsInput.SelectedItems.OfType<InputSavings>().ToList();
        }
    }
}
