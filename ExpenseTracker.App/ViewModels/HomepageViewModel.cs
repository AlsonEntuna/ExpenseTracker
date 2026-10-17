using ExpenseTracker.Data;
using ExpenseTracker.View.Templates;
using ExpenseTracker.Wpf;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace ExpenseTracker.ViewModels
{
    public class HomepageViewModel : ViewModel
    {
        private ExpenseControlViewModel _expenseControlViewModel;
        private ExpenseViewModel _currentExpense;
        private readonly Dictionary<ExpenseViewModel, ExpenseSummary> _summaryCache = new();
        private int _summaryRequest;
        private float _totalExpenses;

        private sealed class ExpenseSummary
        {
            public float Total { get; init; }
            public ObservableCollection<KeyValuePair<string, float>> Categories { get; init; }
            public ObservableCollection<KeyValuePair<string, float>> PaymentChannels { get; init; }
        }

        private sealed class EntrySnapshot
        {
            public string Category { get; init; }
            public string PaymentChannel { get; init; }
            public float Amount { get; init; }
        }

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
                LoadSummaryForCurrentExpense();
            }
        }
#if !DEBUG
        public bool HasCurrentExpense => CurrentExpense?.Expense != null;
#else
        public bool HasCurrentExpense => true;
#endif
        public string ExpenseName => CurrentExpense?.Expense?.Name ?? string.Empty;
        public string ExpenseDescription => CurrentExpense?.Expense?.Description ?? string.Empty;
        public string CycleEndDate => CurrentExpense?.Expense?.EndDate ?? string.Empty;
        public string CurrencySymbol => CurrentExpense?.Expense?.CurrencySymbol ?? string.Empty;
        public int EntryCount => CurrentExpense?.Expense?.Entries?.Count ?? 0;
        public float Budget => CurrentExpense?.Expense?.Budget ?? 0;
        public float TotalExpenses => _totalExpenses;
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
            if (e.PropertyName == nameof(VariableExpense.Entries))
            {
                InvalidateCurrentSummary();
                RaiseExpensePropertiesChanged();
                LoadSummaryForCurrentExpense();
                return;
            }

            RaisePropertyChanged(nameof(ExpenseName));
            RaisePropertyChanged(nameof(ExpenseDescription));
            RaisePropertyChanged(nameof(CycleEndDate));
            RaisePropertyChanged(nameof(CurrencySymbol));
            RaisePropertyChanged(nameof(Budget));
            RaisePropertyChanged(nameof(RemainingBudget));
        }

        private void OnEntriesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (DataEntry entry in e.OldItems)
                    entry.PropertyChanged -= OnEntryPropertyChanged;

            if (e.NewItems != null)
                foreach (DataEntry entry in e.NewItems)
                    entry.PropertyChanged += OnEntryPropertyChanged;

            InvalidateCurrentSummary();
            RaisePropertyChanged(nameof(EntryCount));
            LoadSummaryForCurrentExpense();
        }

        private void OnEntryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DataEntry.Amount)
                || e.PropertyName == nameof(DataEntry.ExpenseCategory)
                || e.PropertyName == nameof(DataEntry.PaymentChannel))
            {
                InvalidateCurrentSummary();
                LoadSummaryForCurrentExpense();
            }
        }

        private void InvalidateCurrentSummary()
        {
            if (CurrentExpense != null)
                _summaryCache.Remove(CurrentExpense);
        }

        private async void LoadSummaryForCurrentExpense()
        {
            int request = ++_summaryRequest;
            ExpenseViewModel expense = CurrentExpense;
            if (expense?.Expense == null)
            {
                ApplySummary(new ExpenseSummary
                {
                    Categories = new ObservableCollection<KeyValuePair<string, float>>(),
                    PaymentChannels = new ObservableCollection<KeyValuePair<string, float>>()
                });
                return;
            }

            if (_summaryCache.TryGetValue(expense, out ExpenseSummary cached))
            {
                ApplySummary(cached);
                return;
            }

            ApplySummary(new ExpenseSummary
            {
                Categories = new ObservableCollection<KeyValuePair<string, float>>(),
                PaymentChannels = new ObservableCollection<KeyValuePair<string, float>>()
            });

            EntrySnapshot[] snapshot = expense.Expense.Entries
                .Select(entry => new EntrySnapshot
                {
                    Category = entry.ExpenseCategory,
                    PaymentChannel = entry.PaymentChannel,
                    Amount = entry.Amount
                })
                .ToArray();

            ExpenseSummary summary = await Task.Run(() => CalculateSummary(snapshot));
            if (request != _summaryRequest || CurrentExpense != expense)
                return;

            _summaryCache[expense] = summary;
            ApplySummary(summary);
        }

        private static ExpenseSummary CalculateSummary(EntrySnapshot[] entries)
        {
            var categories = entries
                .GroupBy(entry => string.IsNullOrWhiteSpace(entry.Category) ? "Uncategorized" : entry.Category)
                .Select(group => new KeyValuePair<string, float>(group.Key, group.Sum(entry => entry.Amount)))
                .OrderByDescending(group => group.Value);
            var paymentChannels = entries
                .GroupBy(entry => string.IsNullOrWhiteSpace(entry.PaymentChannel) ? "Unspecified" : entry.PaymentChannel)
                .Select(group => new KeyValuePair<string, float>(group.Key, group.Sum(entry => entry.Amount)))
                .OrderByDescending(group => group.Value);

            return new ExpenseSummary
            {
                Total = entries.Sum(entry => entry.Amount),
                Categories = new ObservableCollection<KeyValuePair<string, float>>(categories),
                PaymentChannels = new ObservableCollection<KeyValuePair<string, float>>(paymentChannels)
            };
        }

        private void ApplySummary(ExpenseSummary summary)
        {
            _totalExpenses = summary.Total;
            CategoryBreakdown = summary.Categories;
            PaymentChannelBreakdown = summary.PaymentChannels;
            RaisePropertyChanged(nameof(TotalExpenses));
            RaisePropertyChanged(nameof(RemainingBudget));
            RaisePropertyChanged(nameof(CategoryBreakdown));
            RaisePropertyChanged(nameof(PaymentChannelBreakdown));
        }

        private void RaiseExpensePropertiesChanged()
        {
            RaisePropertyChanged(nameof(CurrentExpense));
            RaisePropertyChanged(nameof(HasCurrentExpense));
            RaisePropertyChanged(nameof(ExpenseName));
            RaisePropertyChanged(nameof(ExpenseDescription));
            RaisePropertyChanged(nameof(CycleEndDate));
            RaisePropertyChanged(nameof(CurrencySymbol));
            RaisePropertyChanged(nameof(EntryCount));
            RaisePropertyChanged(nameof(Budget));
            RaisePropertyChanged(nameof(RemainingBudget));
        }
    }
}
