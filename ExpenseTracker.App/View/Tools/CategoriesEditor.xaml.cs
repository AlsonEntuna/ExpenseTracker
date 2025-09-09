using ExpenseTracker.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ExpenseTracker.View.Tools
{
    /// <summary>
    /// Interaction logic for CategoriesEditor.xaml
    /// </summary>
    public partial class CategoriesEditor : Window
    {
        private CategoriesEditorViewModel viewModel;
        public CategoriesEditor()
        {
            InitializeComponent();

            Loaded += (s, e) =>
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
