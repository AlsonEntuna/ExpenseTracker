using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Linq;

using CommunityToolkit.Mvvm.Input;

using ExpenseTracker.Data;
using ExpenseTracker.ViewModels;

using Timer = System.Windows.Forms.Timer;

namespace ExpenseTracker.View
{
    /// <summary>
    /// Interaction logic for VariableExpenseView.xaml
    /// </summary>
    public partial class VariableExpenseView : UserControl
    {
        private Timer _searchDelayTimer;
        private const int _searchDelayTimeout = 500;

        private VariableExpenseViewModel _vm;
        public VariableExpenseView()
        {
            InitializeComponent();
        }

        private void GetDataContext()
        {
            if (_vm != null) return;
            _vm = DataContext as VariableExpenseViewModel;
        }

        private bool UserFilter(object item)
        {
            if (string.IsNullOrEmpty(TxtBox_Search.Text))
            {
                return true;
            }
            else
            {
                return (item as DataEntry).Description.Contains(TxtBox_Search.Text, StringComparison.OrdinalIgnoreCase);
            }
        }

        public ICommand SearchCommand => new RelayCommand(Search);

        private void Search()
        {
            CollectionView collectionView = (CollectionView)CollectionViewSource.GetDefaultView(DataGrid_Expenses.ItemsSource);
            collectionView.Filter = UserFilter;
            CollectionViewSource.GetDefaultView(DataGrid_Expenses.ItemsSource).Refresh();
        }

        private void TxtBox_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtBox_Search.Text)) return;

            if (_searchDelayTimer != null)
                _searchDelayTimer.Stop();

            if (_searchDelayTimer == null)
            {
                _searchDelayTimer = new Timer();
                _searchDelayTimer.Tick += DelaySearch_Tick;
                _searchDelayTimer.Interval = _searchDelayTimeout;
            }

            _searchDelayTimer.Start();
        }

        private void DelaySearch_Tick(object sender, EventArgs e)
        {
            Search();
            if (_searchDelayTimer != null)
                _searchDelayTimer.Stop();
        }

        private void GridSplitter_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var gridSplitter = sender as GridSplitter;

            if (gridSplitter != null)
            {
                ((DataGridCell)gridSplitter.Tag).Column.Width
                    = ((DataGridCell)gridSplitter.Tag).Column.ActualWidth +
                      e.HorizontalChange;
            }
        }

        private void Btn_Open_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AppInstance.Connection.GetEditorViewModel<MainWindowViewModel>().OpenVariableExpense();
        }

        private void Btn_New_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AppInstance.Connection.GetEditorViewModel<MainWindowViewModel>().CreateVariableExpense();
        }

        private void Btn_Tools_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AppInstance.Connection.GetEditorViewModel<MainWindowViewModel>().OpenToolsPanel();
        }

        private void Btn_PiggyBank_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AppInstance.Connection.GetEditorViewModel<MainWindowViewModel>().OpenPiggyBank();
        }

        private void DataGrid_Expenses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetDataContext();
            _vm.SelectedDataEntries = DataGrid_Expenses.SelectedItems.OfType<DataEntry>().ToList();
        }

        private void DataGrid_Expenses_OnCopy(object sender, ExecutedRoutedEventArgs e)
        {
            GetDataContext();
            _vm.CopyEntriesToClipboard();
        }
    }
}
