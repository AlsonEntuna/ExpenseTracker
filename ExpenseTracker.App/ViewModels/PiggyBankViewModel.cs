using ExpenseTracker.Data.Savings;
using ExpenseTracker.Wpf;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;

using CommunityToolkit.Mvvm.Input;

namespace ExpenseTracker.ViewModels
{
    public class PiggyBankViewModel : ViewModel
    {
        public ObservableCollection<SavingsData> Savings
        {
            get
            {
                if (_userSavingsDataService == null)
                    return new ObservableCollection<SavingsData>();
                else
                    return new ObservableCollection<SavingsData>(_userSavingsDataService.UserSavings);
            }
        }

        private SavingsData _selectedSavingsData;
        public SavingsData SelectedSavingsData
        {
            get => _selectedSavingsData;
            set
            {
                SetProperty(ref _selectedSavingsData, value);
                _selectedSavingsData?.Compute();
            }
        }
        public List<InputSavings> SelectedSavingsInput { get; set; }

        private UserSavingsDataService _userSavingsDataService;

        public ICommand CreateNewSavingsCommand => new RelayCommand(CreateNewSavings);
        public ICommand AddSavingsAmountCommand => new RelayCommand(AddSavingsAmount);
        public ICommand EditSavingsCommand => new RelayCommand(EditSavings);
        public ICommand RemoveSavingsCommand => new RelayCommand(RemoveSavings);
        public ICommand RemoveSavingsInputCommand => new RelayCommand(RemoveSavingsInput);
        public ICommand SortByDateCommand => new RelayCommand(SortByDate);

        public PiggyBankViewModel()
        {
            _userSavingsDataService = new UserSavingsDataService();
            _userSavingsDataService.GetOrCreateUserSavingsData();
            if (SelectedSavingsData == null)
            {
                if (Savings != null && Savings.Count > 0)
                {
                    SelectedSavingsData = Savings[0];
                }
            }
        }

        private void CreateNewSavings()
        {
            _userSavingsDataService.AddNewSavingsData();
            RaisePropertyChanged(nameof(Savings));
        }

        public void ForceComputeSavingsData()
        {
            if (SelectedSavingsData != null)
            {
                SelectedSavingsData.Compute();
                RaisePropertyChanged(nameof(SelectedSavingsData));
            }
        }

        private void AddSavingsAmount()
        {
            if (SelectedSavingsData == null)
                return;

            // Add Default value
            SelectedSavingsData.AddInputSavings(0.0f, DateTime.Now.Date.ToShortDateString());
            RaisePropertyChanged(nameof(SelectedSavingsData));
        }

        private void EditSavings()
        {
            if (SelectedSavingsData == null)
                return;

            _userSavingsDataService.EditSavingsData(SelectedSavingsData);
            RaisePropertyChanged(nameof(SelectedSavingsData));
        }
        private void RemoveSavings() 
        {
            if (SelectedSavingsData == null)
                return;

            _userSavingsDataService.RemoveSavingsData(SelectedSavingsData);
            RaisePropertyChanged(nameof(Savings));
        }
        private void RemoveSavingsInput() 
        {
            if (SelectedSavingsInput == null || SelectedSavingsData == null)
                return;

            SelectedSavingsInput.ForEach(input => SelectedSavingsData.RemoveSavingsIinput(input));
            SelectedSavingsInput.Clear();
            ForceComputeSavingsData();
        }

        private void SortByDate()
        {
            if (SelectedSavingsData?.SavingsInput == null)
                return;

            var sorted = SelectedSavingsData.SavingsInput
                .OrderBy(input => input.InputDate)
                .ToList();

            for (int i = 0; i < sorted.Count; ++i)
            {
                int currentIndex = SelectedSavingsData.SavingsInput.IndexOf(sorted[i]);
                if (currentIndex != i)
                    SelectedSavingsData.SavingsInput.Move(currentIndex, i);
            }
        }

        // TODO: make this into an interface
        public void SaveData()
        {
            _userSavingsDataService.SaveUserSavingsData();
        }
    }
}
