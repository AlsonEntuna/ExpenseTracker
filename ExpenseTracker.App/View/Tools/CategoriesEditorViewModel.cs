using ExpenseTracker.Data;
using ExpenseTracker.Tools;
using ExpenseTracker.Wpf;

using CommunityToolkit.Mvvm.Input;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ExpenseTracker.View.Tools
{
    internal class PaymentChannelsViewModel : ViewModel
    {
        private ObservableCollection<string> _paymentChannels;
        public ObservableCollection<string> PaymentChannels => _paymentChannels;

        private string _selectedChannel;
        public string SelectedChannel
        {
            get => _selectedChannel;
            set => SetProperty(ref _selectedChannel, value);
        }

        public ICommand AddPaymentChannelCommand => new RelayCommand(AddPaymentChannel);
        public ICommand RemovePaymentChannelCommand => new RelayCommand(RemovePaymentChannel);
        public PaymentChannelsViewModel(List<string> paymentChannels)
        {
            _paymentChannels = ListUtils.ToObservableCollection(paymentChannels);
        }

        private void AddPaymentChannel()
        {
            throw new NotImplementedException();
        }

        private void RemovePaymentChannel()
        {
            DataHandler.RemovePaymentChannel(SelectedChannel);
        }
    }
    internal class ExpenseCategoriesViewModel : ViewModel
    {
        private ObservableCollection<string> _expenseCategories;
        public ObservableCollection<string> ExpenseCategories => _expenseCategories;

        private string _selectedExpenseCategory;
        public string SelectedExpenseCategory
        {
            get => _selectedExpenseCategory;
            set => SetProperty(ref _selectedExpenseCategory, value);
        }

        public ICommand AddExpenseCategoryCommand => new RelayCommand(AddExpenseCategory);
        public ICommand RemoveExpenseCategoryCommand => new RelayCommand(RemoveExpenseCategory);


        public ExpenseCategoriesViewModel(List<string> expenseCategories)
        {
            _expenseCategories = ListUtils.ToObservableCollection(expenseCategories);
        }

        private void AddExpenseCategory()
        {
            throw new NotImplementedException();
        }
        private void RemoveExpenseCategory()
        {
            DataHandler.RemoveExpenseCategory(_selectedExpenseCategory);
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
