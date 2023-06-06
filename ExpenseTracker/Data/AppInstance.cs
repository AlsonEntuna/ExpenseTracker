using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Data
{
    class AppInstance
    {
        #region Singleton
        private static AppInstance _instance;
        public static AppInstance Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppInstance();
                }
                return _instance;
            }
        }
        #endregion
        public DataCurrency MainCurrency;
        private AppInstance() { }
    }
}
