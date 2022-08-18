using System.Windows;
 using System.Windows.Input;

namespace ExpenseTracker.Dialog
{
    /// <summary>
    /// Interaction logic for NumDialog.xaml
    /// </summary>
    public partial class NumDialog : Window
    {
        public NumDialog()
        {
            InitializeComponent();
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                DialogResult = true;
                Close();
            }
        }

        private void Btn_Done_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
