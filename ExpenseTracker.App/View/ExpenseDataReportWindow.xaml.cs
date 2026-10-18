using System.Windows.Input;
using ExpenseTracker.Wpf.Controls;

namespace ExpenseTracker.View
{
    /// <summary>
    /// Interaction logic for ExpenseDataReportWindow.xaml
    /// </summary>
    public partial class ExpenseDataReportWindow : ExWindow
    {
        public ExpenseDataReportWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = true;
        }
    }
}
