using ExpenseTracker.ExpenseSys;
using ExpenseTracker.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace ExpenseTracker.Data
{

    public static class DataHandler
    {
        public static Configuration Config;
        public static Categories DataCategories;
        private static string _dataFile;
        private static readonly string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ExpenseTracker");
        private static readonly string configFile = Path.Combine(appDataPath, Constants.CONFIG_FILE);
        
        public static void LoadAppConfiguration()
        {
            if (File.Exists(configFile))
            {
                Config = JsonUtils.Deserialize<Configuration>(configFile);
            }
            else
            {
                if (!Directory.Exists(appDataPath))
                {
                    Directory.CreateDirectory(appDataPath);
                }
                Config = Configuration.GenerateConfigFile(configFile);
            }

            LoadCategories();
        }

        public static void SaveAppConfiguration()
        {
            if (!File.Exists(configFile))
            {
                return;
            }

            JsonUtils.Serialize(configFile, Config);
        }

        private static void LoadCategories()
        {
            // TODO: Add support for PaymentChannels
            _dataFile = Path.Combine(appDataPath, Constants.CATEGORIES_FILE);
            if (File.Exists(_dataFile))
            {
                bool legacyData = DetectLegacyData();
                if (legacyData)
                {
                    // We generate new expense categories since it's legacy data we're handling
                    DataCategories.ExpenseCategories = DataUtils.GenerateDefaultCategories();
                    JsonUtils.Serialize(_dataFile, DataCategories);
                }
                else
                {
                    DataCategories = JsonUtils.Deserialize<Categories>(_dataFile);
                }
            }
            else
            {
                DataCategories = new Categories(DataUtils.GenerateDefaultPaymentChannels(), DataUtils.GenerateDefaultCategories());
            }
        }

        /// <summary>
        /// Adds the category to the list of ExpenseCategory.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static bool AddExpenseCategory(string category)
        {
            if (string.IsNullOrEmpty(_dataFile))
            {
                _dataFile = Path.Combine(appDataPath, Constants.CATEGORIES_FILE);
            }
            
            if (!DataCategories.ExpenseCategories.Contains(category))
            {
                DataCategories.ExpenseCategories.Add(category);
                // Serialize immediately
                JsonUtils.Serialize(_dataFile, DataCategories);
                return true;
            }

            return false;
        }

        public static bool AddPaymentChannel(string chanel)
        {
            if (string.IsNullOrEmpty(_dataFile))
            {
                _dataFile = Path.Combine(appDataPath, Constants.CATEGORIES_FILE);
            }

            if (!DataCategories.PaymentChannels.Contains(chanel))
            {
                DataCategories.PaymentChannels.Add(chanel);
                // Serialize immediately
                JsonUtils.Serialize(_dataFile, DataCategories);
                return true;
            }
            return false;
        }

        public static bool DetectLegacyData()
        {
            try
            {
                Categories data = JsonUtils.Deserialize<Categories>(_dataFile);
            }
            catch
            {
                // TODO: legacy data handle here...
                List<string> legacyData = JsonUtils.DeserializeArray<List<string>>(_dataFile);
                
                if (DataCategories == null)
                {
                    DataCategories = new Categories();
                }

                foreach(string paymentChannel in legacyData)
                {
                    AddPaymentChannel(paymentChannel);
                }

                return true;
            }
            return false;
        }
    }
}
