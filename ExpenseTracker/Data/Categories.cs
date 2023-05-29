using System;
using System.Collections.Generic;

namespace ExpenseTracker.Data
{
    [Serializable]
    public class Categories
    {
        public List<string> PaymentChannels { get; set; }
        public List<string> ExpenseCategories { get; set; }
        public Categories()
        {
            PaymentChannels = new List<string>();
            ExpenseCategories = new List<string>();
        }

        public Categories(List<string> paymentChannels, List<string> expenseCategories)
        {
            PaymentChannels = paymentChannels;
            ExpenseCategories = expenseCategories;
        }
    }
}
