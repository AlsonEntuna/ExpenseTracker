using System;
using System.Collections.Generic;
using ExpenseTracker.CurrencyConverter;

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

        public override bool Equals(object obj)
        {
            if (obj is InputSavings other)
            {
                return string.Equals(InputDate, other.InputDate) && Amount == other.Amount;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return InputDate.GetHashCode();
        }
    }

    [Serializable]
    public class SavingsData
    {
        public string SavingsName { get; set; }
        public string SavingsDescription { get; set; }
        public float SavingsTotalAmount { get; set; }
        public float SavingsCurrentAmount { get; set; }
        public float RemainingAmount { get; set; }
        public float SavedPercentage { get; set; }
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

        public void Compute()
        {
            float totalInput = 0;
            foreach(var input in SavingsInput)
            {
                totalInput += input.Amount;
            }
            SavingsCurrentAmount = totalInput;
            RemainingAmount = SavingsTotalAmount - SavingsCurrentAmount;
            SavedPercentage = MathF.Round((SavingsCurrentAmount / SavingsTotalAmount) * 100);
        }

        public void AddInputSavings(float amount, string date)
        {
            InputSavings savings = new InputSavings(amount, date);
            SavingsInput.Add(savings);
            Compute();
        }

        public void RemoveSavingsIinput(InputSavings savingsInput)
        {
            SavingsInput.Remove(savingsInput);
        }
    }
}
