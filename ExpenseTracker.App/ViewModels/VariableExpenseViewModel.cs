﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;

using ExpenseTracker.CurrencyConverter.UI;
using ExpenseTracker.Data;
using ExpenseTracker.Environment;
using ExpenseTracker.Tools;
using ExpenseTracker.View;
using ExpenseTracker.Wpf;
using ExpenseTracker.Wpf.Dialog;


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

                // Initialize the Currency in the Converter, From and To Currencies are the same at startup
                ConverterUIViewModel.FromCurrency = AppInstance.Connection.MainCurrency;
                ConverterUIViewModel.ToCurrency = AppInstance.Connection.MainCurrency;
            }
        }

        private List<DataEntry> _selectedDataEntries;
        public List<DataEntry> SelectedDataEntries
        {
            get => _selectedDataEntries;
            set => SetProperty(ref _selectedDataEntries, value);
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
        public ICommand UpdateEntryConversionCommand => new RelayCommand(UpdateEntryConversion);
        public ICommand CopyEntryCommand => new RelayCommand(CopyEntriesToClipboard);
        public ICommand PasteEntryCommand => new RelayCommand(ProcessEntriesFromClipboard);
        public ICommand SortEntriesCommand => new RelayCommand(SortEntries);
        #endregion

        #region ViewModels
        public CurrencyConverterViewModel ConverterUIViewModel { get; set; }
        #endregion

        public bool IsNewExpense { get; set; }
        public VariableExpenseViewModel() 
        {
            // Register to the app instance connection
            AppInstance.Connection.AddViewModel(this);
            ConverterUIViewModel = new CurrencyConverterViewModel(AppInstance.Connection.CurrConverter);
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
                UpdateEventListeners();
                DataHandler.Config.DataLocation = dialog.FileName;
                DataHandler.SaveAppConfiguration();
                IsNewExpense = false;
            }
        }

        public void SetCurrentDisplayedExpense(VariableExpense expense)
        {
            CurrentDisplayedExpense = expense;
            IsNewExpense = true;
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
            report.DataCurrency = CurrentDisplayedExpense.DataCurrency;
            
            foreach (DataEntry entry in CurrentDisplayedExpense.Entries)
            {
                report.TotalAmount += entry.Amount;
                report.AddCategoryReport(entry);
            }

            report.TotalAmount = (float)Math.Round(report.TotalAmount, 2);
            report.UnPaidAmount = report.TotalAmount;
            report.Savings = (float)Math.Round(CurrentDisplayedExpense.Budget - report.TotalAmount, 2);
            
            return report;
        }

        private void RemoveEntry()
        {
            if (SelectedDataEntries != null)
            { 
                SelectedDataEntries.ForEach(entry => CurrentDisplayedExpense.Entries.Remove(entry));
                SelectedDataEntries.Clear();
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
        }

        private void UpdateEntryConversion()
        {
            foreach (var entry in CurrentDisplayedExpense.Entries)
            {
                if (entry.Currency.Code != AppInstance.Connection.MainCurrency.Code)
                {
                    entry.Amount = entry.OriginalAmount;
                    entry.ConvertToMainCurrency();
                }
            }
        }

        public void CopyEntriesToClipboard()
        {
            if (SelectedDataEntries.Count == 0)
                return;
            string clipboard = JsonUtils.SerializeArrayToString(SelectedDataEntries);

            Clipboard.SetText(clipboard);
        }

        public void ProcessEntriesFromClipboard()
        {
            string clipboard = Clipboard.GetText();
            var entries = JsonUtils.DeserializeArrayFromString<List<DataEntry>>(clipboard);
            if (entries == null)
                return;

            List<DataEntry> _toAdd = new List<DataEntry>();
            if (CurrentDisplayedExpense.Entries.Count != 0)
            {
                foreach (var deserializedEntry in entries)
                {
                    if (!CurrentDisplayedExpense.Entries.Contains(deserializedEntry))
                        _toAdd.Add(deserializedEntry);
                    else
                    {
                        foreach (var entry in CurrentDisplayedExpense.Entries)
                        {
                            if (entry == deserializedEntry)
                            {
                                entry.Amount = deserializedEntry.Amount;
                                entry.Comments = deserializedEntry.Comments;
                            }
                        }
                    }
                }
            }
            else
                _toAdd = entries;

            // Add
            _toAdd.ForEach(CurrentDisplayedExpense.Entries.Add);
        }

        private void SortEntries()
        {
            if (CurrentDisplayedExpense.Entries != null)
            {
                var sortedList = CurrentDisplayedExpense.Entries.OrderBy(f => f.Description).ToList();
                CurrentDisplayedExpense.Entries.Clear();
                foreach (var item in sortedList)
                {
                    CurrentDisplayedExpense.Entries.Add(item);
                }
            }
        }
    }
}
