using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using WpfWrappers.Dialog;
using WPFWrappers;

namespace ExpenseTracker.Data
{
    public class PaidEventArgs : EventArgs
    {
        public bool Paid;
        public float Amount;
    }

    [Serializable]
    public class CategoryReport : ViewModel
    {
        public string CategoryName { get; set; }
        public float Amount { get; set; }
        public string Comments { get; set; }
        private bool _paid;
        public bool Paid
        {
            get => _paid;
            set
            {
                SetProperty(ref _paid, value);
                PaidEventArgs args = new PaidEventArgs()
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

        [NonSerialized]
        public EventHandler<PaidEventArgs> PaidEvent;

        public CategoryReport(string category, float amount)
        {
            CategoryName = category;
            Amount = amount;
        }

        public void AddPartialPayment()
        {
            NumDialog numDialog = new NumDialog()
            {
                Title = "Enter Partial Payment"
            };
            numDialog.ShowDialog();
            if (numDialog.DialogResult == true)
            {
                PartialPayment += float.Parse(numDialog.TextValue);
            }
        }
    }

    [Serializable]
    public class ExpenseDataReport : ViewModel
    {
        private float _totalAmount;
        public float TotalAmount
        {
            get => _totalAmount;
            set => SetProperty(ref _totalAmount, value);
        }

        private float _unpaidAmount;
        public float UnPaidAmount 
        {
            get => _unpaidAmount;
            set => SetProperty(ref _unpaidAmount, value);
        }

        private float _paidAmount;
        public float PaidAmount 
        {
            get => _paidAmount;
            set => SetProperty(ref _paidAmount, value);
        }

        private bool _completed;
        public bool Completed
        {
            get => _completed;
            set => SetProperty(ref _completed, value);
        }
        public List<CategoryReport> CategoryReports { get; set; }
        private List<string> categIds = new List<string>();

        private CategoryReport _selectedReport;
        public CategoryReport SelectedReport
        {
            get => _selectedReport;
            set => SetProperty(ref _selectedReport, value);
        }

        public ICommand AddPartialPaymentCommand => new RelayCommand(AddPartialPayment);

        public ExpenseDataReport()
        {
            CategoryReports = new List<CategoryReport>();

            // Inits
            PaidAmount = 0;
            UnPaidAmount = TotalAmount - PaidAmount;
        }

        private void AddPartialPayment()
        {
            SelectedReport?.AddPartialPayment();
        }

        public void AssessCompletion()
        {
            Completed = CategoryReports.All(f => f.Paid);
        }

        private CategoryReport GetCategoryReport(string category)
        {
            foreach(CategoryReport report in CategoryReports)
            {
                if (report.CategoryName == category)
                {
                    return report;
                }
            }

            return null;
        }

        public void AddCategoryReport(string category, float amount)
        {
            CategoryReport report;
            if (!categIds.Contains(category))
            {
                categIds.Add(category);
                report = new CategoryReport(category, amount);
                CategoryReports.Add(report);
                report.PaidEvent += OnPaidEvent;
            }
            else
            {
                report = GetCategoryReport(category);
                if (report != null)
                {
                    report.Amount += amount;
                }
            }
        }

        public void UpdatePaidEventListeners()
        {
            foreach (CategoryReport report in CategoryReports)
            {
                report.PaidEvent += OnPaidEvent;
            }
        }

        private void OnPaidEvent(object sender, PaidEventArgs e)
        {
            if (e.Paid)
            {
                PaidAmount += e.Amount;
            }
            else
            {
                PaidAmount -= e.Amount;
            }

            UnPaidAmount = TotalAmount - PaidAmount;
        }
    }
}
