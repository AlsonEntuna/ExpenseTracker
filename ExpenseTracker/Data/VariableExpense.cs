using System;
using System.Collections.ObjectModel;
using WPFWrappers;

namespace ExpenseTracker.Data
{
    [Serializable]
    class VariableExpense : ViewModel
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private DateTime _cycleEndDate;
        public DateTime CycleEndDate
        {
            get => _cycleEndDate;
            set => SetProperty(ref _cycleEndDate, value);
        }

        private ObservableCollection<DataEntry> _entires = new ObservableCollection<DataEntry>();
        public ObservableCollection<DataEntry> Entries
        {
            get => _entires;
            set => SetProperty(ref _entires, value);
        }
        public override string ToString()
        {
            return Name;
        }

        public VariableExpense()
        {

        }
    }
}
