using System.Windows;
using System.Windows.Input;

namespace ExpenseTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _vm;
        public MainWindow()
        {
            InitializeComponent();
            _vm = DataContext as MainWindowViewModel;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _vm?.SaveData();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Btn_Open_Click(object sender, RoutedEventArgs e)
        {
            _vm?.OpenVariableExpense();
        }

        private void Btn_New_Click(object sender, RoutedEventArgs e)
        {
            _vm?.CreateVariableExpense();
        }

        private void Btn_Tools_Click(object sender, RoutedEventArgs e)
        {
            _vm?.OpenToolsPanel();
        }
    }
}
