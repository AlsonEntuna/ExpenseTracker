using ExpenseTracker.Data;
using System.Windows;
using System.Windows.Input;
using ExpenseTracker.Wpf.Dialog;

namespace ExpenseTracker.View
{
    /// <summary>
    /// Interaction logic for CreateVariableExpenseWindow.xaml
    /// </summary>
    public partial class CreateExpenseEntry : Window
    {
        public DataEntry Entry { get; set; }
        public CreateExpenseEntry()
        {
            InitializeComponent();
            // Initialize the value
            UpdateCategoryList();
        }

        private void UpdateCategoryList()
        {
            CmbBox_Category.ItemsSource = DataHandler.EntryCategories.ToArray();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TxtBox_Description.Text == string.Empty || TxtBox_Amount.Text == string.Empty)
            {
                MessageBox.Show("Please supply all necessary information", "Input Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            Entry = new DataEntry()
            {
                Description = TxtBox_Description.Text,
                Amount = float.Parse(TxtBox_Amount.Text, System.Globalization.NumberStyles.Float),
                Category = CmbBox_Category.SelectedItem as string
            };

            DialogResult = true;
            Close();
        }

        private void BtnCreateCategory_Click(object sender, RoutedEventArgs e)
        {
            NameDialog dialog = new NameDialog("Enter new Category:");
            if (dialog.ShowDialog() ?? true)
            {
                if (DataHandler.AddCategory(dialog.InputText))
                {
                    UpdateCategoryList();
                }
            }
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
