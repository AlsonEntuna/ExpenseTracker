using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;

using ExpenseTracker.Data;

namespace ExpenseTracker.View
{
    /// <summary>
    /// Interaction logic for VariableExpenseView.xaml
    /// </summary>
    public partial class VariableExpenseView : UserControl
    {
        public VariableExpenseView()
        {
            InitializeComponent();
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
            Search();
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
    }
}
