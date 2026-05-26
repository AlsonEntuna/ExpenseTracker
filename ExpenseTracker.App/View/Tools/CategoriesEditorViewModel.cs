using ExpenseTracker.Data;
using ExpenseTracker.Tools;
using ExpenseTracker.Wpf;

using CommunityToolkit.Mvvm.Input;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;

namespace ExpenseTracker.View.Tools
{
    public class CategoriesEditorViewModel : ViewModel
    {
        public ExpenseCategoriesViewModel ExpenseCategoryVm { get; private set; }
        public PaymentChannelViewModel PaymentChannelVm { get; private set; }

        public CategoriesEditorViewModel()
        {
            // Register to the app instance connection
            AppInstance.Connection.AddViewModel(this);
        }

        // TODO: implement this properly
        public void Initialize()
        {
            ExpenseCategoryVm = new ExpenseCategoriesViewModel(DataHandler.DataCategories.ExpenseCategories);
            PaymentChannelVm = new PaymentChannelViewModel(DataHandler.DataCategories.PaymentChannels);
        }
    }
}
