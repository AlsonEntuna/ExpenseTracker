using ExpenseTracker.Data;
using ExpenseTracker.Tools;
using ExpenseTracker.View;
using ExpenseTracker.View.PiggyBank;
using ExpenseTracker.ViewModels;
using ExpenseTracker.Wpf;
using ExpenseTracker.View.Templates;

#if !DEBUG
using ApplicationUpdater;
#endif

using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

#if !DEBUG
using System.Reflection;
using System.Diagnostics;
#endif

using CommunityToolkit.Mvvm.Input;

namespace ExpenseTracker
{
    class MainWindowViewModel : ViewModel
    {
        #region ViewModels
        private readonly ExpenseControlViewModel _variableExpenseViewModel = new ExpenseControlViewModel();
        public ExpenseControlViewModel ExpenseControlViewModel => _variableExpenseViewModel;
        #endregion

        #region Commands
        public ICommand ExportCategoriesCommand => new RelayCommand(ExportCategories);
        public ICommand ImportCategoriesCommand => new RelayCommand(ImportCategories);
        public ICommand CopyFromCurrentExpenseCommand => new RelayCommand(CopyFromCurrentExpense);
        public ICommand CopyFromOtherExpenseCommand => new RelayCommand(CopyFromOtherExpense);
        public ICommand SaveExpenseCommand => new RelayCommand(SaveExpense);
        #endregion

#if !DEBUG
        private AppClientUpdater _updater;
#endif

        public MainWindowViewModel()
        {
            // Load the data...
            DataHandler.LoadAppConfiguration();
            InitializeData();

            // Register to the app instance connection
            AppInstance.Connection.AddViewModel(this);

#if !DEBUG
            _updater = new AppClientUpdater();
            _updater.InitializeClient("expense_tracker", "AlsonEntuna", "ExpenseTracker");
            Version appVersion = Assembly.GetExecutingAssembly().GetName().Version;
            string ver = $"v{appVersion.Major}.{appVersion.Minor}.{appVersion.Build}";
            _updater.DownloadCompleted += OnDownloadCompleted;
            _updater.CheckForUpdate(ver);
#endif
        }

#if !DEBUG
        private void OnDownloadCompleted(object sender, DownloadInstallerCompleteArgs e)
        {
            DialogResult dialogResult = MessageBox.Show($"A version({e.Version}) of the tool has been downloaded from the release branch.\nDo you want to install it?"
                , "Version Available"
                , MessageBoxButtons.YesNo
                , MessageBoxIcon.Information);
            if (dialogResult == DialogResult.Yes)
            {
                Process.Start(e.InstallerPath);
                App.Current.Shutdown();
            }
        }
#endif

        private void InitializeData()
        {
            if (DataHandler.Config.DataLocations.Count != 0)
            {
                foreach (var dataLocation in DataHandler.Config.DataLocations)
                {
                    try
                    {
                        VariableExpense deserializedData = JsonUtils.Deserialize<VariableExpense>(dataLocation);
                        if (deserializedData == null)
                            return;

                        deserializedData.DetectAndMigrateLegacyData();

                        // Create a new ExpenseViewModel
                        ExpenseViewModel expenseVm = new ExpenseViewModel() { Expense = deserializedData };
                        if (!ExpenseControlViewModel.Expenses.Contains(expenseVm))
                        {
                            ExpenseControlViewModel.AddExpenseToRegistry(expenseVm, dataLocation);
                            ExpenseControlViewModel.UpdateEventListeners(expenseVm);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
        }
        public void OpenToolsPanel()
        {
            ExpenseTrackerTools toolsWindow = new ExpenseTrackerTools
            {
                DataContext = this
            };
            toolsWindow.ShowDialog();
        }
        public void OpenPiggyBank()
        {
            PiggyBankWindow window = new PiggyBankWindow();
            PiggyBankViewModel vm = new PiggyBankViewModel();
            window.DataContext = vm;
            window.Show();
        }
        private void ImportCategories()
        {
            DataHandler.ImportCategories();
        }
        private void ExportCategories()
        {
            DataHandler.ExportCategories();
        }

        public void OpenVariableExpense()
        {
            ExpenseControlViewModel.OpenVariableExpense();
        }

        internal void SaveData()
        {
            ExpenseControlViewModel.SaveCurrentExpenseData();
        }

        public void CreateVariableExpense()
        {
            CreateVariableExpenseWindow window = new();
            window.ShowDialog();
            if (window.DialogResult == true)
            {
                ExpenseViewModel expenseVm = new() { Expense = window.Expense };
                // Only add it directly to the list of expenses because we don't need to store the copy immediately
                // in the registry
                ExpenseControlViewModel.Expenses.Add(expenseVm);
                ExpenseControlViewModel.CurrentExpenseViewModel = expenseVm;
            }
        }

        /// <summary>
        /// Creates a new ExpenseViewModel and copies the model
        /// </summary>
        /// <param name="currentExpense"></param>
        /// <returns></returns>
        private ExpenseViewModel CreateAndCopyVmFromExpense(VariableExpense currentExpense)
        {
            // Create a safe copy of the new expense
            VariableExpense newExpense = new VariableExpense(currentExpense);
            newExpense.Name = $"Copy - {newExpense.Name}";
            newExpense.Description = $"Copy - {newExpense.Description}";

            // Create a new viewmodel based on the copied model
            return new() { Expense = newExpense };
        }
        private void CopyFromCurrentExpense()
        {
            ExpenseViewModel newVm = CreateAndCopyVmFromExpense(ExpenseControlViewModel.CurrentExpenseViewModel.Expense);
            // Only add it directly to the list of expenses because we don't need to store the copy immediately
            // in the registry
            ExpenseControlViewModel.Expenses.Add(newVm);
            ExpenseControlViewModel.CurrentExpenseViewModel = newVm;
        }
        private void CopyFromOtherExpense()
        {
            ExpenseControlViewModel.GetPathFromExpenseDictionary(ExpenseControlViewModel.CurrentExpenseViewModel, out string _path);
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Title = "Select file to copy",
                DefaultExt = "exp",
                Filter = "expense files (*.exp)|*.exp",
                CheckPathExists = true,
                FilterIndex = 2,
                InitialDirectory = Path.GetDirectoryName(_path),
                RestoreDirectory = true
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                VariableExpense expense = JsonUtils.Deserialize<VariableExpense>(dialog.FileName);
                if (expense == null)
                    return;

                ExpenseViewModel newVm = CreateAndCopyVmFromExpense(expense);
                // Only add it directly to the list of expenses because we don't need to store the copy immediately
                // in the registry
                ExpenseControlViewModel.Expenses.Add(newVm);
                ExpenseControlViewModel.CurrentExpenseViewModel = newVm;
            }
        }

        private void SaveExpense()
        {
            AppInstance.Connection.GetEditorViewModel<ExpenseControlViewModel>().SaveCurrentExpenseData();
        }
    }
}
