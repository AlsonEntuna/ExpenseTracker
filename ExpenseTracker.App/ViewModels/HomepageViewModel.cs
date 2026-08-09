using ExpenseTracker.Wpf;
using ExpenseTracker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.ViewModels
{
    public class HomepageViewModel : ViewModel
    {
        public HomepageViewModel()
        {
            // Register to the app instance connection
            AppInstance.Connection.AddViewModel(this);
        }
    }
}
