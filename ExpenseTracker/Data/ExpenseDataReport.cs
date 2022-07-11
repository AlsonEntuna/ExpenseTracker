using System.Collections.Generic;
using System.Linq;

namespace ExpenseTracker.Data
{
    public class CategoryReport
    {
        public string CategoryName { get; set; }
        public float Amount { get; set; }
        public bool Paid { get; set; }

        public CategoryReport(string Category, float Amount)
        {
            CategoryName = Category;
            this.Amount = Amount;
        }
    }

    public class ExpenseDataReport
    {
        public float TotalAmount { get; set; }
        public float UnPaidAmount { get; set; }
        public float PaidAmount { get; set; }
        public bool Completed { get; set; }
        public List<CategoryReport> CategoryReports { get; set; }

        public void AssessCompletion()
        {
            Completed = CategoryReports.All(f => f.Paid);
        }
    }
}
