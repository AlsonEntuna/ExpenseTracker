using System.Windows;
using System.Windows.Input;

namespace ExpenseTracker.View
{
    /// <summary>
    /// Interaction logic for ExpenseDataReportWindow.xaml
    /// </summary>
    public partial class ExpenseDataReportWindow : Window
    {
        public ExpenseDataReportWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
