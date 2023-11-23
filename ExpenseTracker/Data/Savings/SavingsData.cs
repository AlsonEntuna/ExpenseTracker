using ExpenseTracker.CurrencyConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Data.Savings
{
    public class InputSavings
    {
        public float Amount { get; private set; }
        public string InputDate { get; private set; }

        public InputSavings(float amount, string date) 
        {
            Amount = amount;
            InputDate = date;
        }
    }
    public class SavingsData
    {
        private string _savingsName;
        public string SavingsName => _savingsName;

        private float _savingsTotalAmount;
        public float SavingsTotalAmount => _savingsTotalAmount;

        private float _savingsAmount;
        public float SavingsAmount => _savingsAmount;

        private CurrencyInfo _currencyInfo;
        
        private List<InputSavings> _savingsInput;
        public SavingsData(string name) { }
        
        public float GetRemaining()
        {
            float totalInput = 0;
            foreach(var input in _savingsInput)
            {
                totalInput += input.Amount;
            }
            return _savingsTotalAmount -= totalInput;
        }
    }
}
