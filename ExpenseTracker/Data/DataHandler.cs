using ExpenseTracker.ExpenseSys;
using ExpenseTracker.Utils;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ExpenseTracker.Data
{

    public static class DataHandler
    {
        public static Configuration Config;
        public static List<string> EntryCategories = new();
        private static string _categFile;

        public static void LoadAppConfiguration()
        {
            string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string configFile = Path.Combine(assemblyPath, Constants.CONFIG_FILE);
            if (File.Exists(configFile))
                Config = JsonUtils.Deserialize<Configuration>(configFile);
            else
            {
                Config = Configuration.GenerateConfigFile(configFile);
            }

            LoadCategories();
        }

        private static void LoadCategories()
        {
            string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _categFile = Path.Combine(assemblyPath, Constants.CATEGORIES_FILE);
            if (File.Exists(_categFile))
                EntryCategories = JsonUtils.DeserializeArray<List<string>>(_categFile);
            else
            {
                EntryCategories = DataUtils.GenerateDefaultCategories();
                JsonUtils.SerializeArray(_categFile, EntryCategories);
            }
        }

        /// <summary>
        /// Adds the category to the list of ExpenseCategory.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static bool AddCategory(string category)
        {
            if (string.IsNullOrEmpty(_categFile))
            {
                string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                _categFile = Path.Combine(assemblyPath, Constants.CATEGORIES_FILE);
            }
            
            if (!EntryCategories.Contains(category))
            {
                EntryCategories.Add(category);
                // Serialize immediately
                JsonUtils.SerializeArray(_categFile, EntryCategories);
                return true;
            }

            return false;
        }
    }
}
