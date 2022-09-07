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

        private float _budget = 0;
        public float Budget
        {
            get => _budget;
            set => SetProperty(ref _budget, value);
        }

        public string EndDate => CycleEndDate.ToLongDateString();

        private ObservableCollection<DataEntry> _entires = new();
        public ObservableCollection<DataEntry> Entries
        {
            get => _entires;
            set => SetProperty(ref _entires, value);
        }

        public ExpenseDataReport Report { get; set; }
       

        public VariableExpense() { }
        public void AddEntry(DataEntry Entry)
        {
            if (Entry == null)
                return;

            if (!Entries.Contains(Entry))
            {
                Entries.Add(Entry);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
