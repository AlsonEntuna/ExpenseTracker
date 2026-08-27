using ExpenseTracker.Data;
using ExpenseTracker.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.View.Wishlist
{    
    internal class WishlistObjectViewModel : ViewModel
    {
        

        public WishlistObjectViewModel()
        {
            // Register to the app instance connection
            AppInstance.Connection.AddViewModel(this);
        }
    }
}
