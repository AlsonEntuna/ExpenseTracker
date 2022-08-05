using ExpenseTracker.Data;
using ExpenseTracker.ExpenseSys;
using ExpenseTracker.Utils;
using ExpenseTracker.View;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Input;
using WPFWrappers;

namespace ExpenseTracker.ViewModels
{
    class VariableExpenseViewModel : ViewModel
    {
        private VariableExpense _currentDisplayedExpense;
        public VariableExpense CurrentDisplayedExpense
        {
            get => _currentDisplayedExpense;
            set => SetProperty(ref _currentDisplayedExpense, value);
        }

        private DataEntry _selectedDataEntry;
        public DataEntry SelectedDataEntry
        {
            get => _selectedDataEntry;
            set => SetProperty(ref _selectedDataEntry, value);
        }

        public List<string> Categories => DataHandler.EntryCategories;

        #region Commands
        public ICommand AddEntryCommand => new RelayCommand(AddEntry);
        public ICommand SaveVariableExpenseCommand => new RelayCommand(SaveVariableExepense);
        public ICommand GenerateExpenseReportCommand => new RelayCommand(GenerateExpenseReport);
        public ICommand OpenReportCommand => new RelayCommand(OpenReport);
        public ICommand RemoveEntryCommand => new RelayCommand(RemoveEntry);
        #endregion

        public bool IsNewExpense { get; set; }
        public VariableExpenseViewModel()
        {

        }

        private void AddEntry()
        {
            CreateExpenseEntry entryWindow = new CreateExpenseEntry();
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
                DataHandler.Config.DataLocation = dialog.FileName;
                DataHandler.SaveAppConfiguration();
                IsNewExpense = false;
            }
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
            reportWindow.ShowDialog();
        }
        
        private ExpenseDataReport GenerateExpenseDataReport()
        {
            ExpenseDataReport report = new();
            foreach (DataEntry entry in CurrentDisplayedExpense.Entries)
            {
                report.TotalAmount += entry.Amount;
                report.AddCategoryReport(entry.Category, entry.Amount);
            }
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

        public void UpdateEventListeners()
        {
            CurrentDisplayedExpense?.Report?.UpdatePaidEventListeners();
        }
    }
}
