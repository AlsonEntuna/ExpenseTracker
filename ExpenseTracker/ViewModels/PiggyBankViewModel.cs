using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Data.Savings;
using ExpenseTracker.Wpf;
using ExpenseTracker.Wpf.Dialog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
            set => SetProperty(ref _selectedSavingsData, value);
        }

        private UserSavingsDataService _userSavingsDataService;

        public ICommand CreateNewSavingsCommand => new RelayCommand(CreateNewSavings);
        public ICommand AddSavingsAmountCommand => new RelayCommand(AddSavingsAmount);
        public PiggyBankViewModel()
        {
            _userSavingsDataService = new UserSavingsDataService();
            _userSavingsDataService.GetOrCreateUserSavingsData();
        }

        private void CreateNewSavings()
        {
            _userSavingsDataService.AddNewSavingsData();
            RaisePropertyChanged(nameof(Savings));
        }

        private void AddSavingsAmount()
        {
            if (SelectedSavingsData == null) return;

            NumDialog numDialog = new NumDialog("Enter Amount you saved");
            numDialog.ShowDialog();
            if (numDialog.DialogResult == true)
            {
                SelectedSavingsData.AddInputSavings(numDialog.NumValue, DateTime.Now.ToString());
            }
        }

        public void Dispose()
        {
            _userSavingsDataService.SaveUserSavingsData();
        }
    }
}
