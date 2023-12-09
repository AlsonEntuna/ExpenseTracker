using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;

using ExpenseTracker.Data;
using ExpenseTracker.Tools;
using ExpenseTracker.View;
using ExpenseTracker.ViewModels;
using ExpenseTracker.Wpf;

using ApplicationUpdater;
using System.Reflection;
using System.Diagnostics;

namespace ExpenseTracker
{
    class MainWindowViewModel : ViewModel
    {
        #region ViewModels
        private readonly VariableExpenseViewModel variableExpenseViewModel = new VariableExpenseViewModel();
        public VariableExpenseViewModel VariableExpenseViewModel => variableExpenseViewModel;
        #endregion

        #region Commands
        public ICommand ExportCategoriesCommand => new RelayCommand(ExportCategories);
        public ICommand ImportCategoriesCommand => new RelayCommand(ImportCategories);
        public ICommand CopyFromCurrentExpenseCommand => new RelayCommand(CopyFromCurrentExpense);
        public ICommand CopyFromOtherExpenseCommand => new RelayCommand(CopyFromOtherExpense);
        public ICommand SaveExpenseCommand => new RelayCommand(SaveExpense);
        #endregion

        private AppClientUpdater _updater;

        public MainWindowViewModel()
        {
            // Load the data...
            DataHandler.LoadAppConfiguration();
            InitializeData();

            // Register to the app instance connection
            AppInstance.Connection.AddViewModel(this);

            _updater = new AppClientUpdater();
            _updater.InitializeClient("expense_tracker", "AlsonEntuna", "ExpenseTracker");
            Version appVersion = Assembly.GetExecutingAssembly().GetName().Version;
            string ver = $"v{appVersion.Major}.{appVersion.Minor}.{appVersion.Build}";
            _updater.DownloadCompleted += OnDownloadCompleted;
            _updater.CheckForUpdate(ver);
        }

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

        private void InitializeData()
        {
            if (!string.IsNullOrEmpty(DataHandler.Config.DataLocation))
            {
                try
                {
                    VariableExpense deserializedData = JsonUtils.Deserialize<VariableExpense>(DataHandler.Config.DataLocation);
                    if (deserializedData == null)
                        return;
                    VariableExpenseViewModel.CurrentDisplayedExpense = deserializedData;
                    // Detects and migrates old legacy data...
                    variableExpenseViewModel.CurrentDisplayedExpense.DetectAndMigrateLegacyData();
                    variableExpenseViewModel.UpdateEventListeners();
                }
                catch (Exception e)
                {
                    DataHandler.Config.DataLocation = string.Empty;
                    // TODO: have a logging system here...
                }
            }
        }

        public void CreateVariableExpense()
        {
            CreateVariableExpenseWindow window = new();
            window.ShowDialog();
            if (window.DialogResult == true)
            {
                VariableExpenseViewModel.CurrentDisplayedExpense = window.Expense;
                VariableExpenseViewModel.IsNewExpense = true;
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
            VariableExpenseViewModel.OpenVariableExpense();
        }

        internal void SaveData()
        {
            VariableExpenseViewModel.SaveCurrentExpenseData();
        }
        private void CopyFromCurrentExpense()
        {
            var vm = AppInstance.Connection.GetEditorViewModel<VariableExpenseViewModel>();
            if (vm.CurrentDisplayedExpense == null) { return; }

            vm.IsNewExpense = true;
            vm.CurrentDisplayedExpense.Name = "Copy - Replace Me";
            vm.CurrentDisplayedExpense.Description = "Copy - Replace Me";
        }
        private void CopyFromOtherExpense()
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Title = "Select file to copy",
                DefaultExt = "exp",
                Filter = "expense files (*.exp)|*.exp",
                CheckPathExists = true,
                FilterIndex = 2,
                InitialDirectory = Path.GetDirectoryName(DataHandler.Config.DataLocation),
                RestoreDirectory = true
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var copiedDataExpense = JsonUtils.Deserialize<VariableExpense>(dialog.FileName);
                if (copiedDataExpense == null) return;

                copiedDataExpense.DetectAndMigrateLegacyData();
                copiedDataExpense.Name = "Copy - Replace Me";
                copiedDataExpense.Description = "Copy - Replace Me";
                AppInstance.Connection.GetEditorViewModel<VariableExpenseViewModel>().SetCurrentDisplayedExpense(copiedDataExpense);

            }
        }

        private void SaveExpense()
        {
            AppInstance.Connection.GetEditorViewModel<VariableExpenseViewModel>().SaveCurrentExpenseData();
        }
    }
}
