using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Input;

using Microsoft.Toolkit.Mvvm.Input;

using ExpenseTracker.Data;
using ExpenseTracker.ExpenseSys;
using ExpenseTracker.Utils;
using ExpenseTracker.View;
using ExpenseTracker.Wpf;
using ExpenseTracker.Wpf.Dialog;
using System;

namespace ExpenseTracker.ViewModels
{
    class VariableExpenseViewModel : ViewModel
    {
        private VariableExpense _currentDisplayedExpense;
        public VariableExpense CurrentDisplayedExpense
        {
            get => _currentDisplayedExpense;
            set
            {
                SetProperty(ref _currentDisplayedExpense, value);
                // Set the Main Currency
                AppInstance.Connection.MainCurrency = CurrentDisplayedExpense.DataCurrency;
            }
        }

        private DataEntry _selectedDataEntry;
        public DataEntry SelectedDataEntry
        {
            get => _selectedDataEntry;
            set => SetProperty(ref _selectedDataEntry, value);
        }

        public List<string> Categories => DataHandler.DataCategories.ExpenseCategories;
        public List<string> PaymentChannels => DataHandler.DataCategories.PaymentChannels;

        #region Commands
        public ICommand AddEntryCommand => new RelayCommand(AddEntry);
        public ICommand SaveVariableExpenseCommand => new RelayCommand(SaveVariableExepense);
        public ICommand GenerateExpenseReportCommand => new RelayCommand(GenerateExpenseReport);
        public ICommand OpenReportCommand => new RelayCommand(OpenReport);
        public ICommand RemoveEntryCommand => new RelayCommand(RemoveEntry);
        public ICommand EditBudgetCommand => new RelayCommand(EditBudget);
        public ICommand EditExpenseNameCommand => new RelayCommand(EditExpenseName);
        public ICommand EditExpenseDescriptionCommand => new RelayCommand(EditExpenseDescription);
        public ICommand EditDueDateCommand => new RelayCommand(EditDueDate);
        #endregion

        public bool IsNewExpense { get; set; }
        public VariableExpenseViewModel() 
        {
            // Register to the app instance connection
            AppInstance.Connection.AddViewModel(this);
        }

        private void AddEntry()
        {
            CreateExpenseEntry entryWindow = new CreateExpenseEntry(CurrentDisplayedExpense.DataCurrency);
            if (entryWindow.ShowDialog() ?? true)
            {
                CurrentDisplayedExpense.Entries.Add(entryWindow.Entry);
            }
        }

        internal void OpenVariableExpense()
        {
            OpenFileDialog dialog = new()
            {
                Title = "Open Variable Expense File",
                DefaultExt = "exp",
                Filter = "expense files (*.exp)|*.exp",
                CheckPathExists = true,
                FilterIndex = 2,
                InitialDirectory = "C:/",
                RestoreDirectory = true
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                CurrentDisplayedExpense = JsonUtils.Deserialize<VariableExpense>(dialog.FileName);
                // Detect and migrate legacy data
                CurrentDisplayedExpense.DetectAndMigrateLegacyData();
                DataHandler.Config.DataLocation = dialog.FileName;
                DataHandler.SaveAppConfiguration();
                IsNewExpense = false;
            }
        }

        public void SetCurrentDisplayedExpense(VariableExpense expense)
        {
            CurrentDisplayedExpense = expense;
            // Detect and migrate legacy data
            CurrentDisplayedExpense.DetectAndMigrateLegacyData();
            IsNewExpense = false;
        }

        internal void SaveCurrentExpenseData()
        {
            if (string.IsNullOrEmpty(DataHandler.Config.DataLocation) || IsNewExpense)
            {
                SaveFileDialog dialog = new()
                {
                    Title = "Save Expense Data",
                    DefaultExt = "exp",
                    Filter = "expense files (*.exp)|*.exp",
                    CheckPathExists = true,
                    FileName = Constants.DEFAULT_EXPENSE_FILENAME,
                    FilterIndex = 2,
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    JsonUtils.Serialize(dialog.FileName, CurrentDisplayedExpense);
                    DataHandler.Config.DataLocation = dialog.FileName;
                    DataHandler.SaveAppConfiguration();
                    IsNewExpense = false;
                }
            }
            else
            {
                JsonUtils.Serialize(DataHandler.Config.DataLocation, CurrentDisplayedExpense);
            }
        }

        private void GenerateExpenseReport()
        {
            CurrentDisplayedExpense.Report = GenerateExpenseDataReport();
            OpenExpenseReport(CurrentDisplayedExpense.Report);
        }

        private void OpenReport()
        {
            OpenExpenseReport(CurrentDisplayedExpense.Report);
        }

        private void OpenExpenseReport(ExpenseDataReport report)
        {
            ExpenseDataReportViewModel reportVm = new ExpenseDataReportViewModel
            {
                Report = report
            };

            ExpenseDataReportWindow reportWindow = new ExpenseDataReportWindow
            {
                DataContext = reportVm
            };

            //Setting data for the charts
            reportVm.Report.GenerateReportChartData();
            reportWindow.columnChart.DataContext = reportVm.Report.ReportChartData;
            reportWindow.pieChart.DataContext = reportVm.Report.ReportChartData;
            reportWindow.expenseCategoryColumnChart.DataContext = reportVm.Report.ExpenseCategoryChartData;
            reportWindow.expenseCategoryPieChart.DataContext = reportVm.Report.ExpenseCategoryChartData;
            reportWindow.ShowDialog();
        }

        private ExpenseDataReport GenerateExpenseDataReport()
        {
            ExpenseDataReport report = new();
            foreach (DataEntry entry in CurrentDisplayedExpense.Entries)
            {
                report.TotalAmount += entry.Amount;
                report.AddCategoryReport(entry.PaymentChannel, entry.Amount, entry.ExpenseCategory);
                report.GenerateCurrencyReport(entry);
            }
            report.TotalAmount = (float)Math.Round(report.TotalAmount, 2);
            report.UnPaidAmount = report.TotalAmount;
            report.DataCurrency = CurrentDisplayedExpense.DataCurrency;
            return report;
        }

        private void RemoveEntry()
        {
            if (SelectedDataEntry != null)
            {
                CurrentDisplayedExpense.Entries.Remove(SelectedDataEntry);
            }
        }

        private void SaveVariableExepense()
        {
            SaveCurrentExpenseData();
        }

        private void EditBudget()
        {
            NumDialog numDialog = new NumDialog("Enter Your Budget");
            numDialog.ShowDialog();
            if (numDialog.DialogResult == true)
            {
                CurrentDisplayedExpense.Budget = numDialog.NumValue;
            }
        }

        private void EditExpenseName()
        {
            NameDialog nameDialog = new NameDialog("Enter new Expense Name");
            nameDialog.ShowDialog();
            if (nameDialog.DialogResult == true)
            {
                CurrentDisplayedExpense.Name = nameDialog.InputText;
            }
        }

        private void EditExpenseDescription()
        {
            NameDialog nameDialog = new NameDialog("Enter new Expense Description");
            nameDialog.ShowDialog();
            if (nameDialog.DialogResult == true)
            {
                CurrentDisplayedExpense.Description = nameDialog.InputText;
            }
        }

        private void EditDueDate()
        {
            CalendarDialog dialog = new CalendarDialog("Select new date");
            dialog.ShowDialog();
            if (dialog.DialogResult == true) 
            {
                CurrentDisplayedExpense.CycleEndDate = dialog.SeletectDateTimeValue;
            }
        }

        public void UpdateEventListeners()
        {
            CurrentDisplayedExpense?.Report?.UpdatePaidEventListeners();
            //foreach (var entry in  CurrentDisplayedExpense.Entries) 
            //{
            //    entry.ConvertToMainCurrency();
            //}
        }
    }
}
