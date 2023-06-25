using System.Windows;
using System.Windows.Input;

namespace ExpenseTracker.Wpf.Dialog
{
    /// <summary>
    /// Interaction logic for NameDialog.xaml
    /// </summary>
    public partial class NameDialog : Window
    {
        public string InputText => InputTextBox.Text;
        public NameDialog(string title = "")
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(title) ) 
            {
                Title = title;
            }
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                DialogResult = true;
                Close();
            }
        }
    }
}
