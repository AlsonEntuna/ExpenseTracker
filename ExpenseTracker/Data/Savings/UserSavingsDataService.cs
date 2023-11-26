using ExpenseTracker.ExpenseSys;
using ExpenseTracker.Tools;
using ExpenseTracker.View.PiggyBank;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Data.Savings
{
    public class UserSavingsDataService
    {
        private List<SavingsData> _userSavings;
        public List<SavingsData> UserSavings => _userSavings;
        private string _savingsDataPath = Path.Combine(
              PathUtils.AppDataPath(Constants.EXPENSETRACKER)
            , Constants.USER_SAVINGS_FILENAME);
        public void GetOrCreateUserSavingsData()
        {
            if (!File.Exists(_savingsDataPath))
            {
                _userSavings = new List<SavingsData>();
                JsonUtils.SerializeArray(_savingsDataPath, _userSavings);
            }
            else
            {
                _userSavings = JsonUtils.DeserializeArray<List<SavingsData>>(_savingsDataPath);
            }
        }
        public void AddNewSavingsData()
        {
            AddSavingsWindow addSavingsWindow = new AddSavingsWindow();
            addSavingsWindow.ShowDialog();
            if (addSavingsWindow.DialogResult == true)
            {
                _userSavings.Add(addSavingsWindow.NewSavingsData);
            }
        }

        public void SaveUserSavingsData()
        {
            if (_userSavings == null) return;
            JsonUtils.SerializeArray(_savingsDataPath, _userSavings);
        }
    }
}
