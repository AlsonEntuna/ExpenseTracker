using ExpenseTracker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
