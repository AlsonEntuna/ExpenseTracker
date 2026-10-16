using ExpenseTracker.Data;
using ExpenseTracker.Wpf;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

namespace ExpenseTracker.ViewModels
{
    public enum WishlistState
    {
        Items,
        InProgress,
        Achieved
    }

    public class WishlistItemViewModel : ViewModel
    {
        private string _name;
        private double _targetAmount;
        private double _savedAmount;
        private DateTime? _achievedDate;
        private WishlistState _state;

        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public double TargetAmount { get => _targetAmount; set { if (SetProperty(ref _targetAmount, value)) RaisePropertyChanged(nameof(Progress)); } }
        public double SavedAmount { get => _savedAmount; set { if (SetProperty(ref _savedAmount, value)) RaisePropertyChanged(nameof(Progress)); } }
        public double Progress => TargetAmount <= 0 ? 0 : Math.Min(100, SavedAmount / TargetAmount * 100);
        public DateTime? AchievedDate { get => _achievedDate; set => SetProperty(ref _achievedDate, value); }
        public WishlistState State { get => _state; set => SetProperty(ref _state, value); }

        public WishlistItemViewModel(string name, double targetAmount, double savedAmount = 0, WishlistState state = WishlistState.Items, DateTime? achievedDate = null)
        {
            _name = name;
            _targetAmount = targetAmount;
            _savedAmount = savedAmount;
            _state = state;
            _achievedDate = achievedDate;
        }
    }

    public class WishlistViewModel : ViewModel
    {
        public ObservableCollection<WishlistItemViewModel> Items { get; } = new();
        public ObservableCollection<WishlistItemViewModel> InProgressItems { get; } = new();
        public ObservableCollection<WishlistItemViewModel> AchievedItems { get; } = new();

        private string _newItemName;
        private string _newItemTarget;
        public string NewItemName { get => _newItemName; set => SetProperty(ref _newItemName, value); }
        public string NewItemTarget { get => _newItemTarget; set => SetProperty(ref _newItemTarget, value); }
        public ICommand AddItemCommand { get; }

        public WishlistViewModel()
        {
            AppInstance.Connection.AddViewModel(this);
            AddItemCommand = new RelayCommand(AddItem);
            // Prototype examples; wishlist data is not persisted yet.
            Items.Add(new WishlistItemViewModel("Sephora", 500));
            Items.Add(new WishlistItemViewModel("Weekend getaway", 850));
            InProgressItems.Add(new WishlistItemViewModel("PS5", 500, 300, WishlistState.InProgress));
            AchievedItems.Add(new WishlistItemViewModel("Dyson", 500, 500, WishlistState.Achieved, new DateTime(2026, 3, 23)));
        }

        private void AddItem()
        {
            if (string.IsNullOrWhiteSpace(NewItemName) ||
                !double.TryParse(NewItemTarget, NumberStyles.Number, CultureInfo.CurrentCulture, out var target) || target <= 0)
                return;

            Items.Add(new WishlistItemViewModel(NewItemName.Trim(), target));
            NewItemName = string.Empty;
            NewItemTarget = string.Empty;
        }

        public void AddAmount(WishlistItemViewModel item, double amount)
        {
            if (item == null || amount <= 0 || item.State == WishlistState.Achieved) return;
            item.SavedAmount = Math.Min(item.TargetAmount, item.SavedAmount + amount);
            if (item.SavedAmount >= item.TargetAmount)
                MoveItem(item, WishlistState.Achieved);
            else if (item.SavedAmount > 0 && item.State == WishlistState.Items)
                MoveItem(item, WishlistState.InProgress);
        }

        public void MoveItem(WishlistItemViewModel item, WishlistState state)
        {
            if (item == null || item.State == state) return;
            CollectionFor(item.State).Remove(item);
            item.State = state;
            item.AchievedDate = state == WishlistState.Achieved ? DateTime.Today : null;
            CollectionFor(state).Add(item);
        }

        private ObservableCollection<WishlistItemViewModel> CollectionFor(WishlistState state) => state switch
        {
            WishlistState.InProgress => InProgressItems,
            WishlistState.Achieved => AchievedItems,
            _ => Items
        };
    }
}
