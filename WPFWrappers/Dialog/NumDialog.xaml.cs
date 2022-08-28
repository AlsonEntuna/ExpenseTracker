using System.Windows;
 using System.Windows.Input;

namespace ExpenseTracker.Wpf.Dialog
{
    /// <summary>
    /// Interaction logic for NumDialog.xaml
    /// </summary>
    public partial class NumDialog : Window
    {
        public string TextValue => Txt_Value.Text;
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
