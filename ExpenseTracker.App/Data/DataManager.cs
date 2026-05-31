using ExpenseTracker.Data.DataProviders;
using ExpenseTracker.Environment;
using ExpenseTracker.Tools;
using ExpenseTracker.Utils;

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;

using MessageBox = System.Windows.MessageBox;

namespace ExpenseTracker.Data
{
    public class DataManager
    {
        #region Singleton
        private static DataManager _instance;
        public static DataManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DataManager();
                }
                return _instance;
            }
        }
        #endregion

        public Configuration Config;
        public Categories DataCategories;

        private List<IDataProvider> _dataProviders;

        private string _dataFile;
        private string _configFile = Path.Combine(
            PathUtils.AppDataPath(Constants.EXPENSETRACKER)
            , Constants.CONFIG_FILE);
#if DEBUG
        private static string _configDebugPath = Path.Combine(
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            , "_data");
#endif

        // Events
        public EventHandler ProvidersLoadedEvent;
        public EventHandler ProvidersSavedEvent;

        public DataManager()
        {

        }

        public void LoadAppConfiguration()
        {
#if DEBUG
            _configFile = Path.Combine(_configDebugPath, Constants.CONFIG_FILE);
#endif
            if (File.Exists(_configFile))
            {
                Config = JsonUtils.Deserialize<Configuration>(_configFile);
            }
            else
            {
#if DEBUG
                if (!Directory.Exists(_configDebugPath))
                {
                    Directory.CreateDirectory(_configDebugPath);
                }
#else
                if (!Directory.Exists(PathUtils.AppDataPath(Constants.EXPENSETRACKER)))
                {
                    Directory.CreateDirectory(PathUtils.AppDataPath(Constants.EXPENSETRACKER));
                }
#endif
                Config = Configuration.GenerateConfigFile(_configFile);
            }

            LoadCategories();

            // Invoke
            ProvidersLoadedEvent.Invoke(this, EventArgs.Empty);
        }

        public void SaveAppConfiguration()
        {
            if (!File.Exists(_configFile))
            {
                return;
            }

            JsonUtils.Serialize(_configFile, Config);
        }

        private void LoadCategories()
        {
#if DEBUG
            _dataFile = Path.Combine(_configDebugPath, Constants.CATEGORIES_FILE);
#else
            _dataFile = Path.Combine(PathUtils.AppDataPath(Constants.EXPENSETRACKER), Constants.CATEGORIES_FILE);
#endif
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
                // Serialize immediately
                JsonUtils.Serialize(_dataFile, DataCategories);
            }
        }

        /// <summary>
        /// Adds the category to the list of ExpenseCategory.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public bool AddExpenseCategory(string category)
        {
            if (string.IsNullOrEmpty(_instance._dataFile))
            {
                _instance._dataFile = Path.Combine(PathUtils.AppDataPath(Constants.EXPENSETRACKER), Constants.CATEGORIES_FILE);
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

        public bool RemoveExpenseCategory(string category)
        {
            if (string.IsNullOrEmpty(_dataFile))
            {
                _dataFile = Path.Combine(PathUtils.AppDataPath(Constants.EXPENSETRACKER), Constants.CATEGORIES_FILE);
            }

            if (DataCategories.ExpenseCategories.Contains(category))
            {
                DataCategories.ExpenseCategories.Remove(category);
                // Serialize immediately
                // TODO: we have to remove this auto-serialization.
                JsonUtils.Serialize(_dataFile, DataCategories);
                return true;
            }

            return false;
        }

        public bool AddPaymentChannel(string chanel)
        {
            if (string.IsNullOrEmpty(_dataFile))
            {
                _dataFile = Path.Combine(PathUtils.AppDataPath(Constants.EXPENSETRACKER), Constants.CATEGORIES_FILE);
            }

            if (!DataCategories.PaymentChannels.Contains(chanel))
            {
                DataCategories.PaymentChannels.Add(chanel);

                // Serialize immediately
                // TODO: we have to remove this auto-serialization.
                JsonUtils.Serialize(_dataFile, DataCategories);
                return true;
            }
            return false;
        }

        public bool RemovePaymentChannel(string chanel)
        {
            if (string.IsNullOrEmpty(_dataFile))
            {
                _dataFile = Path.Combine(PathUtils.AppDataPath(Constants.EXPENSETRACKER), Constants.CATEGORIES_FILE);
            }

            if (DataCategories.PaymentChannels.Contains(chanel))
            {
                DataCategories.PaymentChannels.Remove(chanel);

                // Serialize immediately
                JsonUtils.Serialize(_dataFile, DataCategories);
                return true;
            }
            return false;
        }

        public bool DetectLegacyData()
        {
            try
            {
                Categories data = JsonUtils.Deserialize<Categories>(_dataFile);
            }
            catch
            {
                List<string> legacyData = JsonUtils.DeserializeArray<List<string>>(_dataFile);

                if (DataCategories == null)
                {
                    DataCategories = new Categories();
                }

                foreach (string paymentChannel in legacyData)
                {
                    AddPaymentChannel(paymentChannel);
                }

                return true;
            }
            return false;
        }

        public void ExportCategories()
        {
            SaveFileDialog dialog = new()
            {
                Title = "Export Categories",
                DefaultExt = ".json",
                Filter = "category files (*.json)|*.json",
                CheckPathExists = true,
                FileName = Constants.CATEGORIES_FILE,
                FilterIndex = 2,
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                JsonUtils.Serialize(dialog.FileName, DataCategories);
                MessageBox.Show("Successfully exported categories data.", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void ImportCategories()
        {
            OpenFileDialog dialog = new()
            {
                Title = "Import Categories",
                DefaultExt = "json",
                Filter = "category files (*.json)|*.json",
                CheckPathExists = true,
                FilterIndex = 2,
                InitialDirectory = "C:/",
                RestoreDirectory = true
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DataCategories = JsonUtils.Deserialize<Categories>(dialog.FileName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
                // Serialize immediately
                JsonUtils.Serialize(_dataFile, DataCategories);
                MessageBox.Show("Successfully imported categories data.", "Import", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}