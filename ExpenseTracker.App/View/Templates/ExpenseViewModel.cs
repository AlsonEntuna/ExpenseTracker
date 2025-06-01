using ExpenseTracker.Data;
using ExpenseTracker.Tools;
using ExpenseTracker.Wpf;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;

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

        // Used in the Control DataTemplate
        public List<string> Categories => DataHandler.DataCategories.ExpenseCategories;
        public List<string> PaymentChannels => DataHandler.DataCategories.PaymentChannels;

        #region Commands
        public ICommand RemoveEntryCommand => new RelayCommand(RemoveEntry);
        public ICommand CopyEntryCommand => new RelayCommand(CopyEntriesToClipboard);
        public ICommand PasteEntryCommand => new RelayCommand(ProcessEntriesFromClipboard);
        public ICommand SortEntriesCommand => new RelayCommand(SortEntries);
        #endregion

        public ExpenseViewModel() { }

        public override string ToString()
        {
            return Expense != null ? Expense.Name : "!Error";
        }

        public override bool Equals(object obj)
        {
            if (obj is ExpenseViewModel _other)
            {
                return Guid.Equals(Expense.UniqueGuid, _other.Expense.UniqueGuid);
            }
            return false;
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

        public void GenerateExpenseDataReport()
        {
            ExpenseDataReport newExpenseReport = new() { DataCurrency = Expense.DataCurrency };

            foreach (DataEntry entry in Expense.Entries)
            {
                newExpenseReport.TotalAmount += entry.Amount;
                newExpenseReport.AddCategoryReport(entry);
            }

            newExpenseReport.TotalAmount = (float)Math.Round(newExpenseReport.TotalAmount, 2);
            newExpenseReport.UnPaidAmount = newExpenseReport.TotalAmount;
            newExpenseReport.Savings = (float)Math.Round(Expense.Budget - newExpenseReport.TotalAmount, 2);

            Expense.Report = newExpenseReport;
        }
    }
}