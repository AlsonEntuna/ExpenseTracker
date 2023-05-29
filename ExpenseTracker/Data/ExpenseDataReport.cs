using ExpenseTracker.Wpf;
using ExpenseTracker.Wpf.Dialog;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;

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
        public string PaymentChannel { get; set; }
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

        private float _outstandingBalance;
        public float OutstandingBalance
        {
            get => _outstandingBalance;
            set => SetProperty(ref _outstandingBalance, value);
        }

        [NonSerialized]
        public EventHandler<PaidEventArgs> PaidEvent;

        public CategoryReport(string paymenChannel, float amount)
        {
            PaymentChannel = paymenChannel;
            Amount = amount;
            OutstandingBalance = Amount;
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

        public DataCurrency DataCurrency { get; set; }
        public string CurrencySymbol
        {
            get
            {
                return DataCurrency != null ? DataCurrency.Symbol : CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
            }
        }

        private ObservableCollection<KeyValuePair<string, int>> _reportChartData;
        public ObservableCollection<KeyValuePair<string, int>> ReportChartData
        {
            get => _reportChartData;
            set => SetProperty(ref _reportChartData, value);
        }

        private ObservableCollection<KeyValuePair<string, int>> _expenseCategoryChartData;
        public ObservableCollection<KeyValuePair<string, int>> ExpenseCategoryChartData
        {
            get => _expenseCategoryChartData;
            set => SetProperty(ref _expenseCategoryChartData, value);
        }

        public Dictionary<string, int> ExpenseCategoryReportCounter;

        public ICommand AddPartialPaymentCommand => new RelayCommand(AddPartialPayment);

        public ExpenseDataReport()
        {
            CategoryReports = new List<CategoryReport>();

            // Inits
            PaidAmount = 0;
        }

        private void AddPartialPayment()
        {
            SelectedReport?.AddPartialPayment();
        }

        public void AssessCompletion()
        {
            Completed = CategoryReports.All(f => f.Paid);
        }

        private CategoryReport GetCategoryReport(string paymenChannel)
        {
            foreach (CategoryReport report in CategoryReports)
            {
                if (report.PaymentChannel == paymenChannel)
                {
                    return report;
                }
            }

            return null;
        }

        public void GenerateReportChartData()
        {
            ReportChartData = new ObservableCollection<KeyValuePair<string, int>>();
            ExpenseCategoryChartData = new ObservableCollection<KeyValuePair<string, int>>();

            foreach (CategoryReport report in CategoryReports)
            {
                try
                {
                    ReportChartData.Add(new KeyValuePair<string, int>(report.PaymentChannel, (int)Math.Round(report.Amount)));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            foreach (KeyValuePair<string, int> expenseCategoryReport in ExpenseCategoryReportCounter)
            {
                try
                {
                    ExpenseCategoryChartData.Add(new KeyValuePair<string, int>(expenseCategoryReport.Key, expenseCategoryReport.Value));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        public void AddCategoryReport(string paymenChannel, float amount, string expenseCategory)
        {
            CategoryReport report;
            if (!categIds.Contains(paymenChannel))
            {
                categIds.Add(paymenChannel);
                report = new CategoryReport(paymenChannel, amount);
                CategoryReports.Add(report);
                report.PaidEvent += OnPaidEvent;
            }
            else
            {
                report = GetCategoryReport(paymenChannel);
                if (report != null)
                {
                    report.Amount += amount;
                }
            }

            if (ExpenseCategoryReportCounter == null)
            {
                ExpenseCategoryReportCounter = new Dictionary<string, int>();
            }

            if (ExpenseCategoryReportCounter.Keys.Contains(expenseCategory))
            {
                ExpenseCategoryReportCounter[expenseCategory] += 1;
            }
            else
            {
                ExpenseCategoryReportCounter.Add(expenseCategory, 1);
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
