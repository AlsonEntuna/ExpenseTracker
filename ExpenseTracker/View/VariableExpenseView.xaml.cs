using ExpenseTracker.Data;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

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
    }
}
