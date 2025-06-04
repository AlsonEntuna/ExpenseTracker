using ExpenseTracker.CurrencyConverter.UI;
using ExpenseTracker.Data;
using ExpenseTracker.Environment;
using ExpenseTracker.Tools;
using ExpenseTracker.View;
using ExpenseTracker.View.Templates;
using ExpenseTracker.Wpf;
using ExpenseTracker.Wpf.Dialog;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;
using Windows.System.Profile;


namespace ExpenseTracker.ViewModels
{
    class ExpenseControlViewModel : ViewModel
    {
        private ExpenseViewModel _currentDisplayedExpense;
        public ExpenseViewModel CurrentExpenseViewModel
        {
            get => _currentDisplayedExpense;
            set
            {
                SetProperty(ref _currentDisplayedExpense, value);
                if (_currentDisplayedExpense != null)
                {
                    // Set the Main Currency
                    AppInstance.Connection.MainCurrency = _currentDisplayedExpense.Expense.DataCurrency;

                    // Initialize the Currency in the Converter, From and To Currencies are the same at startup
                    ConverterUIViewModel.FromCurrency = AppInstance.Connection.MainCurrency;
                    ConverterUIViewModel.ToCurrency = AppInstance.Connection.MainCurrency;
                }
            }
        }

        private ObservableCollection<ExpenseViewModel> _expenses = new();
        public ObservableCollection<ExpenseViewModel> Expenses => _expenses;

        private Dictionary<string, string> _expenseDictionary = new Dictionary<string, string>();

        #region Commands
        public ICommand AddEntryCommand => new RelayCommand(AddEntry);
        public ICommand SaveVariableExpenseCommand => new RelayCommand(SaveVariableExepense);
        public ICommand GenerateExpenseReportCommand => new RelayCommand(GenerateExpenseReport);
        public ICommand OpenReportCommand => new RelayCommand(OpenReport);
        public ICommand EditBudgetCommand => new RelayCommand(EditBudget);
        public ICommand EditExpenseNameCommand => new RelayCommand(EditExpenseName);
        public ICommand EditExpenseDescriptionCommand => new RelayCommand(EditExpenseDescription);
        public ICommand EditDueDateCommand => new RelayCommand(EditDueDate);
        public ICommand UpdateEntryConversionCommand => new RelayCommand(UpdateEntryConversion);
        #endregion

        #region ViewModels
        public CurrencyConverterViewModel ConverterUIViewModel { get; set; }
        #endregion

        public ExpenseControlViewModel()
        {
            // Register to the app instance connection
            AppInstance.Connection.AddViewModel(this);
            ConverterUIViewModel = new CurrencyConverterViewModel(AppInstance.Connection.CurrConverter);
        }

        private void AddEntry()
        {
            CreateExpenseEntry entryWindow = new CreateExpenseEntry(CurrentExpenseViewModel.Expense.DataCurrency);
            if (entryWindow.ShowDialog() ?? true)
            {
                CurrentExpenseViewModel.Expense.Entries.Add(entryWindow.Entry);
            }
        }

        /// <summary>
        /// Adds the Expenses to registry and the list of Expenses in the ViewModel
        /// </summary>
        /// <param name="expenseViewModel"></param>
        /// <param name="absolutePath"></param>
        internal void AddExpenseToRegistry(ExpenseViewModel expenseViewModel, string absolutePath)
        {
            if (!Expenses.Contains(expenseViewModel) && !_expenseDictionary.ContainsKey(expenseViewModel.Expense.UniqueGuid.ToString()))
            {
                _expenseDictionary.Add(expenseViewModel.Expense.UniqueGuid.ToString(), absolutePath);
                Expenses.Add(expenseViewModel);
            }
        }

        internal void RemoveExpenseFromRegistry(ExpenseViewModel expenseViewModel)
        {
            if (expenseViewModel == null)
                return;

            if (Expenses.Contains(expenseViewModel) && _expenseDictionary.ContainsKey(expenseViewModel.Expense.UniqueGuid.ToString()))
            {
                _expenseDictionary.TryGetValue(expenseViewModel.Expense.UniqueGuid.ToString(), out string _pathToRemove);
                DataHandler.Config.DataLocations.Remove(_pathToRemove);
                DataHandler.SaveAppConfiguration();
                _expenseDictionary.Remove(expenseViewModel.Expense.UniqueGuid.ToString());
                Expenses.Remove(expenseViewModel);
                if (CurrentExpenseViewModel == expenseViewModel && Expenses.Count != 0)
                {
                    CurrentExpenseViewModel = Expenses[0];
                }
                else if (CurrentExpenseViewModel == expenseViewModel && Expenses.Count == 0)
                {
                    CurrentExpenseViewModel = null;
                }
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
                VariableExpense newExpense = JsonUtils.Deserialize<VariableExpense>(dialog.FileName);
                
                // Detect and migrate legacy data
                newExpense.DetectAndMigrateLegacyData();
                ExpenseViewModel viewModel = new()
                {
                    Expense = newExpense
                };

                if (!Expenses.Contains(viewModel))
                {
                    AddExpenseToRegistry(viewModel, dialog.FileName);
                    UpdateEventListeners();
                    DataHandler.Config.DataLocations.Add(dialog.FileName);
                    DataHandler.SaveAppConfiguration();
                }
            }
        }

        public void GetPathFromExpenseDictionary(ExpenseViewModel expenseVm, out string path)
        {
            // Check if the path is in the list of the dictionary and if it exists
            bool success = _expenseDictionary.TryGetValue(expenseVm.Expense.UniqueGuid.ToString(), out string _absoluteFilePath);
            path = success ? string.Empty : _absoluteFilePath;
        }

        internal void SaveCurrentExpenseData()
        {
            if (CurrentExpenseViewModel == null)
                return;

            // Check if the path is in the list of the dictionary and if it exists
            bool hasExpense = _expenseDictionary.TryGetValue(CurrentExpenseViewModel.Expense.UniqueGuid.ToString(), out string _absoluteFilePath);
            if (hasExpense && File.Exists(_absoluteFilePath))
            {
                JsonUtils.Serialize(_absoluteFilePath, CurrentExpenseViewModel.Expense);
            }
            else
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
                    bool serializedSucceeded = JsonUtils.Serialize(dialog.FileName, CurrentExpenseViewModel.Expense);
                    if (serializedSucceeded)
                    {
                        _expenseDictionary.Add(CurrentExpenseViewModel.Expense.UniqueGuid.ToString(), dialog.FileName);
                        DataHandler.Config.DataLocations.Add(dialog.FileName);
                        DataHandler.SaveAppConfiguration();
                    }
                }
            }
        }

        private void GenerateExpenseReport()
        {
            CurrentExpenseViewModel.GenerateExpenseDataReport();
            OpenExpenseReport(CurrentExpenseViewModel.Expense.Report);
        }

        private void OpenReport()
        {
            OpenExpenseReport(CurrentExpenseViewModel.Expense.Report);
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
                CurrentExpenseViewModel.Expense.Budget = numDialog.NumValue;
            }
        }

        private void EditExpenseName()
        {
            NameDialog nameDialog = new NameDialog("Enter new Expense Name");
            nameDialog.ShowDialog();
            if (nameDialog.DialogResult == true)
            {
                CurrentExpenseViewModel.Expense.Name = nameDialog.InputText;
            }
        }

        private void EditExpenseDescription()
        {
            NameDialog nameDialog = new NameDialog("Enter new Expense Description");
            nameDialog.ShowDialog();
            if (nameDialog.DialogResult == true)
            {
                CurrentExpenseViewModel.Expense.Description = nameDialog.InputText;
            }
        }

        private void EditDueDate()
        {
            CalendarDialog dialog = new CalendarDialog("Select new date");
            dialog.ShowDialog();
            if (dialog.DialogResult == true)
            {
                CurrentExpenseViewModel.Expense.CycleEndDate = dialog.SeletectDateTimeValue;
            }
        }

        public void UpdateEventListeners()
        {
            CurrentExpenseViewModel?.Expense.Report?.UpdatePaidEventListeners();
        }

        private void UpdateEntryConversion()
        {
            foreach (var entry in CurrentExpenseViewModel.Expense.Entries)
            {
                if (entry.Currency.Code != AppInstance.Connection.MainCurrency.Code)
                {
                    entry.Amount = entry.OriginalAmount;
                    entry.ConvertToMainCurrency();
                }
            }
        }
    }
}