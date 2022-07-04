using ExpenseTracker.Data;
using System.Windows;
using WPFWrappers.Dialog;

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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // TODO: build the data here....
            Entry = new DataEntry()
            {
                Description = TxtBox_Description.Text,
                Amount = float.Parse(TxtBox_Amount.Text, System.Globalization.NumberStyles.Float),
                Category = CmbBox_Category.SelectedItem as DataCategory
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
    }
}
