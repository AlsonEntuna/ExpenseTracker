using System.Collections.Generic;

namespace ExpenseTracker.Utils
{
    public static class DataUtils
    {
        public static List<string> GenerateDefaultCategories()
        {
            List<string> categories = new List<string>()
            {
                "Food",
                "Shopping",
                "Bills",
                "Transportation",
                "Rent",
                "Mortgage",
                "Utilities",
            };

            return categories;
        }

        public static List<string> GenerateDefaultPaymentChannels()
        {
            List<string> channels = new List<string>()
            {
                "Credit Card",
                "Loan",
                "Mortgage",
            };

            return channels;
        }
    }
}
