using System.IO;
using System.Collections.Generic;

using ExpenseTracker.ExpenseSys;
using ExpenseTracker.Tools;
using ExpenseTracker.View.PiggyBank;

namespace ExpenseTracker.Data.Savings
{
    public class UserSavingsDataService
    {
        private List<SavingsData> _userSavings;
        public List<SavingsData> UserSavings => _userSavings;
#if DEBUG
        private string _savingsDataPath = Path.Combine(
              Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "_data")
            , Constants.USER_SAVINGS_FILENAME);
#else
        private string _savingsDataPath = Path.Combine(
             PathUtils.AppDataPath(Constants.EXPENSETRACKER)
           , Constants.USER_SAVINGS_FILENAME);
#endif

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

        public void EditSavingsData(SavingsData savingsToEdit)
        {
            AddSavingsWindow addSavingsWindow = new AddSavingsWindow();
            addSavingsWindow.Txt_SavingsName.Text = savingsToEdit.SavingsName;
            addSavingsWindow.Txt_Description.Text = savingsToEdit.SavingsDescription;
            addSavingsWindow.Txt_Amount.Text = savingsToEdit.SavingsTotalAmount.ToString();
            addSavingsWindow.Combo_Currency.SelectedItem = savingsToEdit.SavingsCurrency;
            addSavingsWindow.Combo_Currency.IsEnabled = false;
            addSavingsWindow.Btn_AddSavings.Content = "Update";

            addSavingsWindow.ShowDialog();
            if (addSavingsWindow.DialogResult == true)
            {
                savingsToEdit.SavingsName = addSavingsWindow.NewSavingsData.SavingsName;
                savingsToEdit.SavingsDescription = addSavingsWindow.NewSavingsData.SavingsDescription;
                savingsToEdit.SavingsTotalAmount = addSavingsWindow.NewSavingsData.SavingsTotalAmount;
            }
        }

        public void RemoveSavingsData(SavingsData savingsToRemove)
        {
            _userSavings.Remove(savingsToRemove);
        }

        public void SaveUserSavingsData()
        {
            if (_userSavings == null) return;
            JsonUtils.SerializeArray(_savingsDataPath, _userSavings);
        }
    }
}
