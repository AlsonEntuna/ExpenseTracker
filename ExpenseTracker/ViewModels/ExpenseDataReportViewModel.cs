using ExpenseTracker.Data;
using ExpenseTracker.Wpf;

namespace ExpenseTracker.ViewModels
{
    class ExpenseDataReportViewModel : ViewModel
    {
        private ExpenseDataReport _report;
        public ExpenseDataReport Report
        {
            get => _report;
            set => SetProperty(ref _report, value);
        }
        public ExpenseDataReportViewModel()
        {

        }
    }
}
