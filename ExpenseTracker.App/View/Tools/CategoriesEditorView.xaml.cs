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

            Unloaded += (_, _) =>
            {
                SaveHadlers();
            };
        }

        private void SaveHadlers()
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
