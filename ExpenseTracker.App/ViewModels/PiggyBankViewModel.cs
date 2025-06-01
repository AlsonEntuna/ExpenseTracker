using ExpenseTracker.Data.Savings;
using ExpenseTracker.Wpf;
using ExpenseTracker.Wpf.Dialog;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

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
            if (SelectedSavingsData == null) return;

            NumDialog numDialog = new NumDialog("Enter the amount you saved");
            numDialog.ShowDialog();
            if (numDialog.DialogResult == true)
            {
                SelectedSavingsData.AddInputSavings(numDialog.NumValue, DateTime.Now.Date.ToShortDateString());
                RaisePropertyChanged(nameof(SelectedSavingsData));
            }
        }

        private void EditSavings()
        {
            if (SelectedSavingsData == null) return;
            _userSavingsDataService.EditSavingsData(SelectedSavingsData);
            RaisePropertyChanged(nameof(SelectedSavingsData));
        }
        private void RemoveSavings() 
        {
            if (SelectedSavingsData == null) return;
            _userSavingsDataService.RemoveSavingsData(SelectedSavingsData);
            RaisePropertyChanged(nameof(Savings));
        }
        private void RemoveSavingsInput() 
        {
            if (SelectedSavingsInput == null || SelectedSavingsData == null) return;

            SelectedSavingsInput.ForEach(input => SelectedSavingsData.RemoveSavingsIinput(input));
            SelectedSavingsInput.Clear();
            ForceComputeSavingsData();
        }

        public void Dispose()
        {
            _userSavingsDataService.SaveUserSavingsData();
        }
    }
}
