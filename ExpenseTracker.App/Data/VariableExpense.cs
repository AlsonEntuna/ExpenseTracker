﻿using ExpenseTracker.CurrencyConverter;
using ExpenseTracker.Wpf;

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;


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

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
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

        public Guid UniqueGuid { get; set; }

        public VariableExpense()
        {
            // Generate Unique Guid
            UniqueGuid = Guid.NewGuid();
        }

        public VariableExpense(VariableExpense other)
        {
            // Make sure that we generate a new unique GUID
            UniqueGuid = Guid.NewGuid();
            Name = other.Name;
            Description = other.Description;
            Entries = other.Entries;
            DataCurrency = other.DataCurrency;
            CycleEndDate = other.CycleEndDate;
            EndDate = other.EndDate;
            Budget = other.Budget;
        }

        public void AddEntry(DataEntry Entry)
        {
            if (Entry != null && !Entries.Contains(Entry))
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

            // Check Guid
            if (UniqueGuid == Guid.Empty)
            {
                UniqueGuid = Guid.NewGuid();
            }
        }

        public override string ToString() => Name;
    }
}
