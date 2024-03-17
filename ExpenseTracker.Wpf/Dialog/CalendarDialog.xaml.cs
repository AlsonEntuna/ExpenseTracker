using System;
using System.Windows;

namespace ExpenseTracker.Wpf.Dialog
{
    /// <summary>
    /// Interaction logic for CalendarDialog.xaml
    /// </summary>
    public partial class CalendarDialog : Window
    {
        public DateTime SeletectDateTimeValue => DPicker_ExpenseDate.SelectedDate.Value;
        public CalendarDialog(string title = "")
        {
            InitializeComponent();
            DPicker_ExpenseDate.SelectedDate = System.DateTime.Now;
            if (!string.IsNullOrEmpty(title))
            {
                Title = title;
            }
        }

        private void Btn_Done_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
