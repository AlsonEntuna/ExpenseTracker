using ExpenseTracker.CurrencyConverter;
using System;
using System.Collections.Generic;

namespace ExpenseTracker.Data.Savings
{
    [Serializable]
    public class InputSavings
    {
        public float Amount { get; set; }
        public string InputDate { get; set; }

        public InputSavings(float amount, string date) 
        {
            Amount = amount;
            InputDate = date;
        }
    }

    [Serializable]
    public class SavingsData
    {
        public string SavingsName { get; set; }
        public string SavingsDescription { get; set; }
        public float SavingsTotalAmount { get; set; }
        public float SavingsCurrentAmount { get; set; }
        public CurrencyInfo SavingsCurrency { get; set; }

        public List<InputSavings> SavingsInput { get; set; } = new List<InputSavings>();
        public SavingsData(string name, string description, float amount, CurrencyInfo currency)
        {
            SavingsName = name;
            SavingsDescription = description;
            SavingsTotalAmount = amount;
            SavingsCurrency = currency;
        }

        public override string ToString()
        {
            return SavingsName;
        }

        public float GetRemainingAmount()
        {
            float totalInput = 0;
            foreach(var input in SavingsInput)
            {
                totalInput += input.Amount;
            }
            return SavingsTotalAmount -= totalInput;
        }

        public float GetTotalSavedAmount()
        {
            float total = 0;
            foreach (var input in SavingsInput)
                total += input.Amount;

            return total;
        }

        public void AddInputSavings(float amount, string date)
        {
            InputSavings savings = new InputSavings(amount, date);
            SavingsInput.Add(savings);
        }
    }
}
