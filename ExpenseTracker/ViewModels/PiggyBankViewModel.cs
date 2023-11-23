using ExpenseTracker.Data.Savings;
using ExpenseTracker.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.ViewModels
{
    public class PiggyBankViewModel : ViewModel
    {
        private ObservableCollection<UserSavingsData> _savings;
        public ObservableCollection<UserSavingsData> Savings
        {
            get => _savings;
            set => SetProperty(ref _savings, value);
        }

        public PiggyBankViewModel() { }
    }
}
