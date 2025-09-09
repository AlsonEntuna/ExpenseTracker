using ExpenseTracker.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ExpenseTracker.View.Tools
{
    /// <summary>
    /// Interaction logic for PaymentChannelControl.xaml
    /// </summary>
    public partial class PaymentChannelControl : UserControl
    {
        private PaymentChannelsViewModel viewModel;
        public PaymentChannelControl()
        {
            InitializeComponent();

            // Only when loaded then we get the DataContext
            Loaded += (s, e) =>
            {
                viewModel = DataContext as PaymentChannelsViewModel;
                viewModel.NewPaymentChannelEvent += OnNewPaymentChannelEvent;
            };
        }

        private async void OnNewPaymentChannelEvent(object sender, string e)
        {
            await Dispatcher.Yield(DispatcherPriority.Render);
            List<TextBox> textBoxes = WpfHelpers.FindVisualChildrenOfTypeRecursive<TextBox>(ListView_PaymenChannels);
            foreach (TextBox textBox in textBoxes)
            {
                if (textBox.Text == e)
                {
                    textBox.Focus();
                    textBox.CaretIndex = 0;
                    textBox.SelectAll();
                    break;
                }
            }
        }
    }
}
