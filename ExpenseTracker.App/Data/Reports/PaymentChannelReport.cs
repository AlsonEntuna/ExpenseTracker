using ExpenseTracker.Wpf.Dialog;
using ExpenseTracker.Wpf;
using ExpenseTracker.Data.Events;

using System;
using System.Windows.Documents;
using System.Collections.Generic;

namespace ExpenseTracker.Data.Reports
{
    [Serializable]
    public class ExpenseCategoryBreakdown
    {
        private string _category;
        public string Category => _category;
        private float _totalAmount;
        public float TotalAmount => _totalAmount;
        public ExpenseCategoryBreakdown(string category)
        {
            _category = category;
        }

        public void AddAmount(float amount)
        {
            _totalAmount += amount;
        }
    }

    [Serializable]
    public class PaymentChannelReport : ViewModel
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

        private List<ExpenseCategoryBreakdown> _categoryBreakdown;
        public List<ExpenseCategoryBreakdown> CategoryBreakdown
        {
            get => _categoryBreakdown;
            set => SetProperty(ref _categoryBreakdown, value);
        }

        [NonSerialized]
        public EventHandler<PaidEventArgs> PaidEvent;

        public PaymentChannelReport(string paymentChannel, float amount)
        {
            PaymentChannel = paymentChannel;
            Amount = amount;
            OutstandingBalance = Amount;
            _categoryBreakdown = new List<ExpenseCategoryBreakdown>();
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

        public void AddToBreakdownData(string category, float amount)
        {
            // TODO: implement
            ExpenseCategoryBreakdown breakdown = CategoryBreakdown.Find(c => c.Category == category);
            if (breakdown == null)
            {
                breakdown = new ExpenseCategoryBreakdown(category);
                _categoryBreakdown.Add(breakdown);
            }
            breakdown.AddAmount(amount);
        }
    }
}
