using System.Collections.Generic;

namespace ExpenseTracker.Utils
{
    public static class DataUtils
    {
        public static List<string> GenerateDefaultCategories()
        {
            List<string> categ = new List<string>()
            {
                "Citibank",
                "BPI Credit",
                "GCredit",
                "BPI-Car",
                "Metrobank",
                "Eastwest"
            };

            return categ;
        }
    }
}
