using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Data;
using ExpenseTracker.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ExpenseTracker.View.Tools
{
    internal class PaymentChannelItem : ViewModel
    {
        private string _paymentChannelName;
        public string PaymentChannelName
        {
            get => _paymentChannelName;
            set => SetProperty(ref _paymentChannelName, value);
        }

        private bool _editing = false;
        public bool Editing
        {
            get => _editing;
            set => SetProperty(ref _editing, value);
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
    internal class PaymentChannelViewModel : ViewModel, IDataHandler
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

        public PaymentChannelViewModel(List<string> paymentChannels)
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
}
