using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

using ExpenseTracker.CurrencyConverter;
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
            set
            {
                SetProperty(ref _cycleEndDate, value);
                EndDate = CycleEndDate.ToLongDateString();
            }
        }
        private string _endDate;
        public string EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        private float _budget = 0;
        public float Budget
        {
            get => _budget;
            set => SetProperty(ref _budget, value);
        }

        public CurrencyInfo DataCurrency { get; set; }
        // NOTE: This is a fail-safe option.
        public CultureInfo Currency => CultureInfo.CurrentCulture;

        public string CurrencySymbol
        {
            get
            {
                return DataCurrency != null ? DataCurrency.Symbol : CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
            }
        }

        private ObservableCollection<DataEntry> _entires = new();
        public ObservableCollection<DataEntry> Entries
        {
            get => _entires;
            set => SetProperty(ref _entires, value);
        }

        private ExpenseDataReport _report;

        public ExpenseDataReport Report
        {
            get => _report;
            set => SetProperty(ref _report, value);
        }


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

        public void DetectAndMigrateLegacyData()
        {
            bool hasLegacyData = Entries.Any(f => f.Category != null && f.Category.Length != 0);
            if (hasLegacyData)
            {
                foreach (var entry in Entries)
                {
                    entry.PaymentChannel = entry.Category;
                    entry.Category = null;
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
