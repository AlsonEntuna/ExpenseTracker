using ExpenseTracker.Data;
using ExpenseTracker.Tools;
using ExpenseTracker.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.View.Tools
{
    internal class PaymentChannelsViewModel : ViewModel
    {
        private ObservableCollection<string> _paymentChannels;
        public ObservableCollection<string> PaymentChannels => _paymentChannels;
        public PaymentChannelsViewModel(List<string> paymentChannels)
        {
            _paymentChannels = ListUtils.ToObservableCollection(paymentChannels);
        }
    }
    internal class ExpenseCategoriesViewModel : ViewModel
    {
        private ObservableCollection<string> _expenseCategories;
        public ObservableCollection<string> ExpenseCategories => _expenseCategories;
        public ExpenseCategoriesViewModel(List<string> expenseCategories)
        {
            _expenseCategories = ListUtils.ToObservableCollection(expenseCategories);
        }
    }
    internal class CategoriesEditorViewModel : ViewModel
    {
        public ExpenseCategoriesViewModel ExpenseCategoryVm { get; private set; }
        public PaymentChannelsViewModel PaymentChannelVm { get; private set; }

        public CategoriesEditorViewModel()
        {
            ExpenseCategoryVm = new ExpenseCategoriesViewModel(DataHandler.DataCategories.ExpenseCategories);
            PaymentChannelVm = new PaymentChannelsViewModel(DataHandler.DataCategories.PaymentChannels);
        }
    }
}
