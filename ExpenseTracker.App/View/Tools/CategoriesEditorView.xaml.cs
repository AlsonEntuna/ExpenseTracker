using System.Windows.Controls;

namespace ExpenseTracker.View.Tools
{
    /// <summary>
    /// Interaction logic for CategoriesEditor.xaml
    /// </summary>
    public partial class CategoriesEditorView : UserControl
    {
        private CategoriesEditorViewModel viewModel;
        public CategoriesEditorView()
        {
            InitializeComponent();

            Loaded += (_, _) =>
            {
                viewModel = DataContext as CategoriesEditorViewModel;
            };
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            {
                if (viewModel.PaymentChannelVm is IDataHandler _handler)
                    _handler.Save();
            }

            {
                if (viewModel.ExpenseCategoryVm is IDataHandler _handler)
                    _handler.Save();
            }
        }
    }
}
