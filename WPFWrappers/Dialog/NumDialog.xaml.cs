using System.Text.RegularExpressions;
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
        public float NumValue => float.Parse(Txt_Value.Text);
        public NumDialog(string title = "")
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(title))
            {
                Title = title;
            }
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

        private static readonly Regex _regex = new Regex("[+-]?([0-9]*[.])?[0-9]+");
        private static bool IsNumeric(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void Txt_Value_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumeric(e.Text);
        }
    }
}
