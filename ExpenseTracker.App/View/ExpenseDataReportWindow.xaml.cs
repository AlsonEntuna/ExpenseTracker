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

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = true;
        }
    }
}
