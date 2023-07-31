using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Data.Events;
using ExpenseTracker.Data.Reports;
using ExpenseTracker.Wpf;

namespace ExpenseTracker.Data
{
    [Serializable]
    public class ExpenseDataReport : ViewModel
    {
        private float _totalAmount;
        public float TotalAmount
        {
            get => _totalAmount;
            set => SetProperty(ref _totalAmount, value);
        }

        private float _savings;
        public float Savings
        {
            get => _savings;
            set => SetProperty(ref _savings, value);
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
        public string CurrencySymbol => DataCurrency != null ? DataCurrency.Symbol : CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;

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

        private ObservableCollection<CurrencyData> _currencyReport;
        public ObservableCollection<CurrencyData> CurrencyReport 
        {
            get => _currencyReport;
            set => SetProperty(ref _currencyReport, value);
        }

        public Dictionary<string, int> ExpenseCategoryReportCounter;
        public Dictionary<string, Dictionary<string, float>> AltCurrencyBreakdown { get; set; }
        #region Commands
        [Newtonsoft.Json.JsonIgnore]
        public ICommand AddPartialPaymentCommand => new RelayCommand(AddPartialPayment);
        #endregion

        public ExpenseDataReport()
        {
            CategoryReports = new List<CategoryReport>();
            CurrencyReport = new ObservableCollection<CurrencyData>();
            AltCurrencyBreakdown = new Dictionary<string, Dictionary<string, float>>();

            // Inits
            PaidAmount = 0;
            Savings = 0;
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

        public void AddCategoryReport(DataEntry entry)
        {
            CategoryReport report;
            if (!categIds.Contains(entry.PaymentChannel))
            {
                categIds.Add(entry.PaymentChannel);
                report = new CategoryReport(entry.PaymentChannel, entry.Amount);
                CategoryReports.Add(report);
                report.PaidEvent += OnPaidEvent;
            }
            else
            {
                report = GetCategoryReport(entry.PaymentChannel);
                if (report != null)
                {
                    report.Amount += entry.Amount;
                    report.OutstandingBalance += entry.Amount;
                }
            }


            // Expense Category Report
            if (ExpenseCategoryReportCounter == null)
                ExpenseCategoryReportCounter = new Dictionary<string, int>();

            if (ExpenseCategoryReportCounter.Keys.Contains(entry.ExpenseCategory))
                ExpenseCategoryReportCounter[entry.ExpenseCategory] += 1;
            else
                ExpenseCategoryReportCounter.Add(entry.ExpenseCategory, 1);

            // AltCurrencyBreakdown
            if (DataCurrency.Code != entry.Currency.Code)
            {
                // TODO: refactor to Dictionary<string, class>();
                if (AltCurrencyBreakdown.Keys.Contains(entry.Currency.Code))
                {
                    if (AltCurrencyBreakdown[entry.Currency.Code].Keys.Contains(entry.PaymentChannel))
                        AltCurrencyBreakdown[entry.Currency.Code][entry.PaymentChannel] += entry.OriginalAmount;
                    else
                        AltCurrencyBreakdown[entry.Currency.Code].Add(entry.PaymentChannel, entry.OriginalAmount);
                }
                else
                {
                    var breakdownDict = new Dictionary<string, float>() { { entry.PaymentChannel, entry.OriginalAmount } };
                    AltCurrencyBreakdown.Add(entry.Currency.Code, breakdownDict);
                }
            }

            GenerateCurrencyReport(entry);
        }

        public void GenerateCurrencyReport(DataEntry entry)
        {
            if (AppInstance.Connection.MainCurrency.Code == entry.Currency.Code)
                return;

            CurrencyData data = new CurrencyData(entry.Currency, entry.OriginalAmount);
            if (!CurrencyReport.Contains(data))
                CurrencyReport.Add(data);
            else
            {
                foreach(var curData in CurrencyReport)
                {
                    if (string.Equals(curData.Currency.Code, data.Currency.Code))
                    {
                        curData.Amount = (float)Math.Round(curData.Amount += data.Amount, 2);
                        return;
                    }
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
