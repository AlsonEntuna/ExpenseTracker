using System.Windows;

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
    }
}
