using System;
using ExpenseTracker.Wpf.Dialog;
using ExpenseTracker.Wpf;
using ExpenseTracker.Data.Events;

namespace ExpenseTracker.Data.Reports
{
    [Serializable]
    public class CategoryReport : ViewModel
    {
        public string PaymentChannel { get; set; }
        private float _amount;
        public float Amount
        {
            get => (float)MathF.Round(_amount, 2);
            set => SetProperty(ref _amount, value);
        }
        public string Comments { get; set; }
        private bool _paid;
        public bool Paid
        {
            get => _paid;
            set
            {
                SetProperty(ref _paid, value);
                PaidEventArgs args = new()
                { Amount = this.Amount, Paid = this.Paid };
                PaidEvent?.Invoke(this, args);
            }
        }

        private float _partialPayment;
        public float PartialPayment
        {
            get => _partialPayment;
            set => SetProperty(ref _partialPayment, value);
        }

        private float _outstandingBalance;
        public float OutstandingBalance
        {
            get => (float)MathF.Round(_outstandingBalance, 2);
            set => SetProperty(ref _outstandingBalance, value);
        }

        [NonSerialized]
        public EventHandler<PaidEventArgs> PaidEvent;

        public CategoryReport(string paymentChannel, float amount)
        {
            PaymentChannel = paymentChannel;
            Amount = amount;
            OutstandingBalance = Amount;
        }

        public void AddPartialPayment()
        {
            NumDialog numDialog = new NumDialog("Enter Partial Payment");
            numDialog.ShowDialog();
            if (numDialog.DialogResult == true)
            {
                PartialPayment += numDialog.NumValue;
            }

            // Compute the outstanding balance
            OutstandingBalance = Amount - PartialPayment;
            if (OutstandingBalance == 0)
            {
                Paid = true;
            }
        }
    }
}
