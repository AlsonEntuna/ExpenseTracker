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
    interface IDataHandler
    {
        public void Save();
    }
    internal class PaymentChannelItem : ViewModel
    {
        private string _paymentChannelName;
        public string PaymentChannelName
        { 
            get => _paymentChannelName;
            set => SetProperty(ref _paymentChannelName, value);
        }
        
        private bool _isEditing = false;
        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        public Guid Id { get; private set; }
        public PaymentChannelItem() { }
        public PaymentChannelItem(string name)
        {
            PaymentChannelName = name;
            Id = Guid.NewGuid();
        }

        public override bool Equals(object obj)
        {
            if (obj is PaymentChannelItem otherItem)
            {
                return otherItem.Id == Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode()
                , Id.GetHashCode()
                , PaymentChannelName.GetHashCode());
        }
    }
    internal class PaymentChannelsViewModel : ViewModel, IDataHandler
    {
        private ObservableCollection<PaymentChannelItem> _paymentChannels = new();
        public ObservableCollection<PaymentChannelItem> PaymentChannels => _paymentChannels;

        private PaymentChannelItem _selectedChannel;
        public PaymentChannelItem SelectedChannel
        {
            get => _selectedChannel;
            set => SetProperty(ref _selectedChannel, value);
        }

        public ICommand AddPaymentChannelCommand => new RelayCommand(AddPaymentChannel);
        public ICommand RemovePaymentChannelCommand => new RelayCommand(RemovePaymentChannel);

        public EventHandler<string> NewPaymentChannelEvent;

        private Dictionary<string, Guid> _cachedIdMappings = new Dictionary<string, Guid>();

        public PaymentChannelsViewModel(List<string> paymentChannels)
        {
            // Cache the PaymentChannel Name <> ID
            foreach (string channelName in paymentChannels)
            {
                PaymentChannelItem pItem = new PaymentChannelItem(channelName);
                _cachedIdMappings[channelName] = pItem.Id;
                _paymentChannels.Add(pItem);
            }

            // Register to the app instance connection
            AppInstance.Connection.AddViewModel(this);
        }

        private void AddPaymentChannel()
        {
            string channelName = "New Channel";
            SelectedChannel = new PaymentChannelItem(channelName);
            //DataHandler.AddPaymentChannel(SelectedChannel);
            _paymentChannels.Add(SelectedChannel);

            NewPaymentChannelEvent?.Invoke(this, channelName);
        }

        private void RemovePaymentChannel()
        {
            DataHandler.RemovePaymentChannel(SelectedChannel.PaymentChannelName);
            // TODO: improve and not to remove from both ends
            PaymentChannels.Remove(SelectedChannel);
        }

        public void Save()
        {
            DataHandler.DataCategories.PaymentChannels.Clear();
            foreach (PaymentChannelItem pItem in _paymentChannels)
            {
                DataHandler.AddPaymentChannel(pItem.PaymentChannelName);
            }
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
            // Register to the app instance connection
            AppInstance.Connection.AddViewModel(this);
        }

        private void AddExpenseCategory()
        {
            string expenseCategory = "New Category";
            SelectedExpenseCategory = expenseCategory;
            DataHandler.AddExpenseCategory(SelectedExpenseCategory);
        }
        private void RemoveExpenseCategory()
        {
            // TODO: improve internal handling of copies
            ExpenseCategories.Remove(_selectedExpenseCategory);
            DataHandler.RemoveExpenseCategory(_selectedExpenseCategory);
        }

        private void Save()
        {

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
            // Register to the app instance connection
            AppInstance.Connection.AddViewModel(this);
        }
    }
}
