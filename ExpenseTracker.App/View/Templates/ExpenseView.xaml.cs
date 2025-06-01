using ExpenseTracker.Data;
using System.Linq;
using System.Windows.Controls;

namespace ExpenseTracker.View.Templates
{
    public partial class ExpenseView : UserControl
    {
        private ExpenseViewModel _vm;
        public ExpenseView()
        {
            InitializeComponent();

        }
        private void GetDataContext()
        {
            if (_vm != null) return;
            _vm = DataContext as ExpenseViewModel;
        }

        private void GridSplitter_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var gridSplitter = sender as GridSplitter;

            if (gridSplitter != null)
            {
                ((DataGridCell)gridSplitter.Tag).Column.Width
                    = ((DataGridCell)gridSplitter.Tag).Column.ActualWidth +
                      e.HorizontalChange;
            }
        }

        private void ExpenseDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetDataContext();
            _vm.SelectedDataEntries = ExpenseDataGrid.SelectedItems.OfType<DataEntry>().ToList();
        }

        private void ExpenseDataGrid_OnCopy(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            GetDataContext();
            _vm.CopyEntriesToClipboard();
        }
    }
}