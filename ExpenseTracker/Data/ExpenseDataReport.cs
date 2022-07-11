using System;
using System.Collections.Generic;
using System.Linq;
using WPFWrappers;

namespace ExpenseTracker.Data
{
    public class PaidEventArgs : EventArgs
    {
        public bool Paid;
        public float Amount;
    }
    public class CategoryReport : ViewModel
    {
        public string CategoryName { get; set; }
        public float Amount { get; set; }
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

        public EventHandler<PaidEventArgs> PaidEvent;

        public CategoryReport(string category, float amount)
        {
            CategoryName = category;
            Amount = amount;
        }
    }

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

        public ExpenseDataReport()
        {
            CategoryReports = new List<CategoryReport>();

            // Inits
            PaidAmount = 0;
            UnPaidAmount = TotalAmount - PaidAmount;
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
