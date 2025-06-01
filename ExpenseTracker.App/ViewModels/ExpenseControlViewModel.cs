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
using System.Windows.Forms;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;
using System.IO;


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
                // Set the Main Currency
                AppInstance.Connection.MainCurrency = _currentDisplayedExpense.Expense.DataCurrency;

                // Initialize the Currency in the Converter, From and To Currencies are the same at startup
                ConverterUIViewModel.FromCurrency = AppInstance.Connection.MainCurrency;
                ConverterUIViewModel.ToCurrency = AppInstance.Connection.MainCurrency;
            }
        }

        private ObservableCollection<ExpenseViewModel> _expenses = new();
        public ObservableCollection<ExpenseViewModel> Expenses
        {
            get => _expenses;
            set
            {
                SetProperty(ref _expenses, value);
            }
        }

        private Dictionary<string, string> _expenseDictionary = new Dictionary<string, string>();

        private List<DataEntry> _selectedDataEntries;
        public List<DataEntry> SelectedDataEntries
        {
            get => _selectedDataEntries;
            set => SetProperty(ref _selectedDataEntries, value);
        }

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

        public bool IsNewExpense { get; set; }
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

        internal void AddExpense(ExpenseViewModel expenseViewModel, string absolutePath)
        {
            if (!Expenses.Contains(expenseViewModel) && !_expenseDictionary.ContainsKey(expenseViewModel.Expense.UniqueGuid.ToString()))
            {
                _expenseDictionary.Add(expenseViewModel.Expense.UniqueGuid.ToString(), absolutePath);
                Expenses.Add(expenseViewModel);
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
                    //Expenses.Add(viewModel);
                    AddExpense(viewModel, dialog.FileName);
                    UpdateEventListeners();
                    // TODO: we need to change the data location from string to List<string> to contain more than 1 string
                    //DataHandler.Config.DataLocation = dialog.FileName;
                    DataHandler.Config.DataLocations.Add(dialog.FileName);
                    DataHandler.SaveAppConfiguration();
                    IsNewExpense = false;
                }
            }
        }

        /// <summary>
        /// Is used when the tool creates an expense using the copy method
        /// </summary>
        /// <param name="expenseVm"></param>
        public void SetCurrentExpenseViewModel(ExpenseViewModel expenseVm)
        {
            CurrentExpenseViewModel = expenseVm;
            IsNewExpense = true;
        }

        internal void SaveCurrentExpenseData()
        {
            // If it's a new file
            if (IsNewExpense)
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
                    JsonUtils.Serialize(dialog.FileName, CurrentExpenseViewModel.Expense);
                    // TODO: fix this
                    //DataHandler.Config.DataLocation = dialog.FileName;
                    //DataHandler.SaveAppConfiguration();
                    _expenseDictionary.Add(CurrentExpenseViewModel.Expense.UniqueGuid.ToString(), dialog.FileName);
                    // TODO: fix the logic of the copying
                    DataHandler.Config.DataLocations.Add(dialog.FileName);
                    IsNewExpense = false;
                }
            }
            else
            {
               if (CurrentExpenseViewModel != null)
               {
                    bool success = _expenseDictionary.TryGetValue(CurrentExpenseViewModel.Expense.UniqueGuid.ToString(), out string _absoluteFilePath);
                    if (success && File.Exists(_absoluteFilePath))
                    {
                        JsonUtils.Serialize(_absoluteFilePath, CurrentExpenseViewModel.Expense);
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
