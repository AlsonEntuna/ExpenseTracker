using System;

namespace ExpenseTracker.Data.Events
{
    public class PaidEventArgs : EventArgs
    {
        public bool Paid;
        public float Amount;
    }
}
