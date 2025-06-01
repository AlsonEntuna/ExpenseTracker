using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using ExpenseTracker.Data;
using ExpenseTracker.Tools;
using ExpenseTracker.Wpf;

namespace ExpenseTracker.View.Templates
{
    public class ExpenseViewModel : ViewModel
    {
        private VariableExpense _Expense;
        public VariableExpense Expense
        {
            get => _Expense;
            set
            {
                SetProperty(ref _Expense, value);
                // Set the Main Currency
                AppInstance.Connection.MainCurrency = Expense.DataCurrency;
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
        public ICommand RemoveEntryCommand => new RelayCommand(RemoveEntry);
        public ICommand CopyEntryCommand => new RelayCommand(CopyEntriesToClipboard);
        public ICommand PasteEntryCommand => new RelayCommand(ProcessEntriesFromClipboard);
        public ICommand SortEntriesCommand => new RelayCommand(SortEntries);
        #endregion

        public ExpenseViewModel()
        {

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
            if (Expense.Entries.Count != 0)
            {
                foreach (var deserializedEntry in entries)
                {
                    if (!Expense.Entries.Contains(deserializedEntry))
                        _toAdd.Add(deserializedEntry);
                    else
                    {
                        foreach (var entry in Expense.Entries)
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
            _toAdd.ForEach(Expense.Entries.Add);
        }
        private void RemoveEntry()
        {
            if (SelectedDataEntries != null)
            {
                SelectedDataEntries.ForEach(entry => Expense.Entries.Remove(entry));
                SelectedDataEntries.Clear();
            }
        }
        private void SortEntries()
        {
            if (Expense.Entries != null)
            {
                var sortedList = Expense.Entries.OrderBy(f => f.Description).ToList();
                Expense.Entries.Clear();
                foreach (var item in sortedList)
                {
                    Expense.Entries.Add(item);
                }
            }
        }
    }
}