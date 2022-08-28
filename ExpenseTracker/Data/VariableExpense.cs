using System;
using System.Collections.ObjectModel;
using ExpenseTracker.Wpf;

namespace ExpenseTracker.Data
{
    [Serializable]
    public class VariableExpense : ViewModel
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _desc;
        public string Description
        {
            get => _desc;
            set => SetProperty(ref _desc, value);
        }

        private DateTime _cycleEndDate;
        public DateTime CycleEndDate
        {
            get => _cycleEndDate;
            set => SetProperty(ref _cycleEndDate, value);
        }

        public string EndDate => CycleEndDate.ToLongDateString();

        private ObservableCollection<DataEntry> _entires = new ObservableCollection<DataEntry>();
        public ObservableCollection<DataEntry> Entries
        {
            get => _entires;
            set => SetProperty(ref _entires, value);
        }

        public ExpenseDataReport Report { get; set; }
        public override string ToString()
        {
            return Name;
        }

        public VariableExpense()
        {

        }

        public void AddEntry(DataEntry Entry)
        {
            if (Entry == null)
                return;

            if (!Entries.Contains(Entry))
            {
                Entries.Add(Entry);
            }
        }
    }
}
