using ExpenseTracker.Data;

using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ExpenseTracker.View.Templates
{
    public partial class ExpenseView : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ExpenseVm"
                , typeof(ExpenseViewModel)
                , typeof(ExpenseView)
                , new PropertyMetadata(null));

        public ExpenseViewModel ExpenseVm
        {
            get => (ExpenseViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public ExpenseView()
        {
            InitializeComponent();
        }

        private void GridSplitter_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            GridSplitter gridSplitter = sender as GridSplitter;

            if (gridSplitter != null)
            {
                ((DataGridCell)gridSplitter.Tag).Column.Width
                    = ((DataGridCell)gridSplitter.Tag).Column.ActualWidth +
                      e.HorizontalChange;
            }
        }

        private void ExpenseDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ExpenseVm != null)
            {
                ExpenseVm.SelectedDataEntries = ExpenseDataGrid.SelectedItems.OfType<DataEntry>().ToList();
            }
        }

        private void ExpenseDataGrid_OnCopy(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (ExpenseVm != null)
            {
                ExpenseVm.CopyEntriesToClipboard();
            }
        }
    }
}