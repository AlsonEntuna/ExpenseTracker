using ExpenseTracker.Data;
using ExpenseTracker.ExpenseSys;
using ExpenseTracker.Utils;
using ExpenseTracker.View;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Input;
using WPFWrappers;
using WPFWrappers.Command;

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

        public List<string> Categories => DataHandler.EntryCategories;

        #region Commands
        public ICommand AddEntryCommand => new RelayCommand(f => { AddEntry(); }, f => true);
        public ICommand GenerateExpenseDataCommand => new RelayCommand(f => { GenerateExpenseData(); }, f => true);
        public ICommand SaveVariableExpenseCommand => new RelayCommand(f => { SaveVariableExepense(); }, f => true);
        #endregion

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

        private void GenerateExpenseData()
        {

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
            }
        }

        internal void SaveCurrentExpenseData()
        {
            if (string.IsNullOrEmpty(DataHandler.Config.DataLocation))
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
                }
            }
            else
            {
                JsonUtils.Serialize(DataHandler.Config.DataLocation, CurrentDisplayedExpense);
            }
        }

        private void SaveVariableExepense()
        {
            SaveCurrentExpenseData();
        }
    }
}
