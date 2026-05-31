using ExpenseTracker.Wpf;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ExpenseTracker.View.Tools
{
    /// <summary>
    /// Interaction logic for PaymentChannelControl.xaml
    /// </summary>
    public partial class PaymentChannelControl : UserControl
    {
        private PaymentChannelViewModel viewModel;
        public PaymentChannelControl()
        {
            InitializeComponent();

            // Only when loaded then we get the DataContext
            Initialized += (s, e) =>
            {
                viewModel = DataContext as PaymentChannelViewModel;
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
