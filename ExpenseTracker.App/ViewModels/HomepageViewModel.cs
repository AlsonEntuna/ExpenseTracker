using ExpenseTracker.Data;
using ExpenseTracker.View.Templates;
using ExpenseTracker.Wpf;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace ExpenseTracker.ViewModels
{
    public class HomepageViewModel : ViewModel
    {
        private ExpenseControlViewModel _expenseControlViewModel;
        private ExpenseViewModel _currentExpense;

        public ExpenseViewModel CurrentExpense
        {
            get => _currentExpense;
            private set
            {
                if (_currentExpense == value)
                    return;

                UnsubscribeFromExpense(_currentExpense);
                _currentExpense = value;
                SubscribeToExpense(_currentExpense);
                RaiseExpensePropertiesChanged();
            }
        }

        public bool HasCurrentExpense => CurrentExpense?.Expense != null;
        public string ExpenseName => CurrentExpense?.Expense?.Name ?? string.Empty;
        public string ExpenseDescription => CurrentExpense?.Expense?.Description ?? string.Empty;
        public string CycleEndDate => CurrentExpense?.Expense?.EndDate ?? string.Empty;
        public string CurrencySymbol => CurrentExpense?.Expense?.CurrencySymbol ?? string.Empty;
        public int EntryCount => CurrentExpense?.Expense?.Entries?.Count ?? 0;
        public float Budget => CurrentExpense?.Expense?.Budget ?? 0;
        public float TotalExpenses => CurrentExpense?.Expense?.Entries?.Sum(entry => entry.Amount) ?? 0;
        public float RemainingBudget => Budget - TotalExpenses;
        public ObservableCollection<KeyValuePair<string, float>> CategoryBreakdown { get; private set; } = new();
        public ObservableCollection<KeyValuePair<string, float>> PaymentChannelBreakdown { get; private set; } = new();

        public HomepageViewModel()
        {
            AppInstance.Connection.AddViewModel(this);
        }

        public HomepageViewModel(ExpenseControlViewModel expenseControlViewModel) : this()
        {
            _expenseControlViewModel = expenseControlViewModel;
            _expenseControlViewModel.PropertyChanged += OnExpenseControlPropertyChanged;
            CurrentExpense = _expenseControlViewModel.CurrentExpenseViewModel;
        }

        private void OnExpenseControlPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ExpenseControlViewModel.CurrentExpenseViewModel))
                CurrentExpense = _expenseControlViewModel.CurrentExpenseViewModel;
        }

        private void SubscribeToExpense(ExpenseViewModel expense)
        {
            if (expense?.Expense == null)
                return;

            expense.Expense.PropertyChanged += OnExpensePropertyChanged;
            expense.Expense.Entries.CollectionChanged += OnEntriesChanged;
            foreach (DataEntry entry in expense.Expense.Entries)
                entry.PropertyChanged += OnEntryPropertyChanged;
        }

        private void UnsubscribeFromExpense(ExpenseViewModel expense)
        {
            if (expense?.Expense == null)
                return;

            expense.Expense.PropertyChanged -= OnExpensePropertyChanged;
            expense.Expense.Entries.CollectionChanged -= OnEntriesChanged;
            foreach (DataEntry entry in expense.Expense.Entries)
                entry.PropertyChanged -= OnEntryPropertyChanged;
        }

        private void OnExpensePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseExpensePropertiesChanged();
        }

        private void OnEntriesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (DataEntry entry in e.OldItems)
                    entry.PropertyChanged -= OnEntryPropertyChanged;

            if (e.NewItems != null)
                foreach (DataEntry entry in e.NewItems)
                    entry.PropertyChanged += OnEntryPropertyChanged;

            RaiseExpensePropertiesChanged();
        }

        private void OnEntryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DataEntry.Amount))
                RaiseExpensePropertiesChanged();
        }

        private void RefreshBreakdowns()
        {
            var entries = CurrentExpense?.Expense?.Entries ?? new ObservableCollection<DataEntry>();

            CategoryBreakdown = new ObservableCollection<KeyValuePair<string, float>>(entries
                .GroupBy(entry => string.IsNullOrWhiteSpace(entry.ExpenseCategory) ? "Uncategorized" : entry.ExpenseCategory)
                .Select(group => new KeyValuePair<string, float>(group.Key, group.Sum(entry => entry.Amount)))
                .OrderByDescending(group => group.Value));

            PaymentChannelBreakdown = new ObservableCollection<KeyValuePair<string, float>>(entries
                .GroupBy(entry => string.IsNullOrWhiteSpace(entry.PaymentChannel) ? "Unspecified" : entry.PaymentChannel)
                .Select(group => new KeyValuePair<string, float>(group.Key, group.Sum(entry => entry.Amount)))
                .OrderByDescending(group => group.Value));
        }

        private void RaiseExpensePropertiesChanged()
        {
            RefreshBreakdowns();
            RaisePropertyChanged(nameof(CurrentExpense));
            RaisePropertyChanged(nameof(HasCurrentExpense));
            RaisePropertyChanged(nameof(ExpenseName));
            RaisePropertyChanged(nameof(ExpenseDescription));
            RaisePropertyChanged(nameof(CycleEndDate));
            RaisePropertyChanged(nameof(CurrencySymbol));
            RaisePropertyChanged(nameof(EntryCount));
            RaisePropertyChanged(nameof(Budget));
            RaisePropertyChanged(nameof(TotalExpenses));
            RaisePropertyChanged(nameof(RemainingBudget));
            RaisePropertyChanged(nameof(CategoryBreakdown));
            RaisePropertyChanged(nameof(PaymentChannelBreakdown));
        }
    }
}
