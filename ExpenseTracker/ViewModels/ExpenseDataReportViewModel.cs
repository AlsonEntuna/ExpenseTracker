using ExpenseTracker.Data;
using WPFWrappers;

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
