using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Data.Wishlist
{
    internal class WishlistObject
    {
        public enum WishlistState
        {
            NotStarted,
            InProgress,
            OnHold,
            Completed,
            _COUNT
        }

        private WishlistState _state;
    }
}
