using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Data.DataProviders
{
    internal interface IDataProvider
    {
        bool Save();
        bool Load();
    }
}
