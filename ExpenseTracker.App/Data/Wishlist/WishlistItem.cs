using ExpenseTracker.Wpf;

using System;

namespace ExpenseTracker.Data.Wishlist
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
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        private double _targetAmount;
        public double TargetAmount
        { 
            get => _targetAmount;
            set
            { 
                if (SetProperty(ref _targetAmount, value))
                {
                    RaisePropertyChanged(nameof(Progress));
                }
            } 
        }
        private double _savedAmount;
        public double SavedAmount
        { 
            get => _savedAmount;
            set
            { 
                if (SetProperty(ref _savedAmount, value))
                {
                    RaisePropertyChanged(nameof(Progress));
                }
            }
        }
        private DateTime? _achievedDate;
        public DateTime? AchievedDate 
        {
            get => _achievedDate;
            set => SetProperty(ref _achievedDate, value);
        }
        private WishlistState _state;

        public double Progress => TargetAmount <= 0 ? 0 : Math.Min(100, SavedAmount / TargetAmount * 100);
        public WishlistState State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }

        public WishlistItemViewModel(string name
            , double targetAmount
            , double savedAmount = 0
            , WishlistState state = WishlistState.Items
            , DateTime? achievedDate = null)
        {
            _name = name;
            _targetAmount = targetAmount;
            _savedAmount = savedAmount;
            _state = state;
            _achievedDate = achievedDate;
        }
    }
}