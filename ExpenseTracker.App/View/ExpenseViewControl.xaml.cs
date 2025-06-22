using ExpenseTracker.Data;
using ExpenseTracker.View.Templates;
using ExpenseTracker.ViewModels;
using ExpenseTracker.Wpf;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

using CommunityToolkit.Mvvm.Input;

using Timer = System.Windows.Forms.Timer;
using ExpenseTracker.View.Tools;

namespace ExpenseTracker.View
{
    /// <summary>
    /// Interaction logic for ExpenseViewControl.xaml
    /// </summary>
    public partial class ExpenseViewControl : UserControl
    {
        private Timer _searchDelayTimer;
        private const int _searchDelayTimeout = 500;

        private ExpenseControlViewModel _vm;
        private Window parentWindow = Application.Current.MainWindow;
        public ExpenseViewControl()
        {
            InitializeComponent();
            parentWindow.SizeChanged += OnParentSizeChanged;
            _vm = AppInstance.Connection.GetEditorViewModel<ExpenseControlViewModel>();
        }

        private void OnParentSizeChanged(object sender, SizeChangedEventArgs e)
        {
            TabControlExpenses.MaxHeight = Grid_DataEntryView.ActualHeight - 100;
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
            int index = TabControlExpenses.SelectedIndex;
            TabItem tabItem = TabControlExpenses.ItemContainerGenerator.ContainerFromIndex(index) as TabItem;
            if (tabItem == null)
            {
                // Early return because we have to have a ContentPresenter present as it is dependent on the TabItem
                return;
            }


            ContentPresenter contentPresenter = WpfHelpers.FindVisualChildOfType<ContentPresenter>(TabControlExpenses);
            if (contentPresenter == null)
                return;

            // Get the ExpenseView UserControl as it is a template
            ExpenseView expenseView = VisualTreeHelper.GetChild(contentPresenter, 0) as ExpenseView;
            CollectionView collectionView = (CollectionView)CollectionViewSource.GetDefaultView(expenseView.ExpenseDataGrid.ItemsSource);
            collectionView.Filter = UserFilter;
            CollectionViewSource.GetDefaultView(expenseView.ExpenseDataGrid.ItemsSource).Refresh();
        }
        private void TxtBox_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
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

        private void Btn_Open_Click(object sender, RoutedEventArgs e)
        {
            AppInstance.Connection.GetEditorViewModel<MainWindowViewModel>().OpenVariableExpense();
        }

        private void Btn_New_Click(object sender, RoutedEventArgs e)
        {
            AppInstance.Connection.GetEditorViewModel<MainWindowViewModel>().CreateVariableExpense();
        }

        private void Btn_Tools_Click(object sender, RoutedEventArgs e)
        {
            AppInstance.Connection.GetEditorViewModel<MainWindowViewModel>().OpenToolsPanel();
        }

        private void Btn_PiggyBank_Click(object sender, RoutedEventArgs e)
        {
            AppInstance.Connection.GetEditorViewModel<MainWindowViewModel>().OpenPiggyBank();
        }

        private void TabControlExpenses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabControlExpenses.SelectedItem is ExpenseViewModel _expenseVm)
            {
                _vm.CurrentExpenseViewModel = _expenseVm;
            }

        }

        private void Button_CloseExpenseItem_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null)
                return;

            // Find the parent TabItem
            TabItem tabItem = WpfHelpers.FindVisualParentOfType<TabItem>(button);
            if (tabItem != null)
            {
                if (tabItem.Content is ExpenseViewModel _expenseVm)
                {
                    _vm.RemoveExpenseFromRegistry(_expenseVm);
                }
            }
        }

        private void Btn_CategTools_Click(object sender, RoutedEventArgs e)
        {
            CategoriesEditor categoriesEditor = new CategoriesEditor();
            CategoriesEditorViewModel categoriesViewModel = new CategoriesEditorViewModel();
            categoriesEditor.DataContext = categoriesViewModel;
            categoriesEditor.ShowDialog();
        }
    }
}