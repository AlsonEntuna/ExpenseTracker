using ExpenseTracker.Utils;
using System;
using System.IO;
using System.Reflection;

namespace ExpenseTracker.Data
{

    public class DataHandler
    {
        private static readonly Lazy<DataHandler> lazy = new Lazy<DataHandler>(() => new DataHandler());
        public static DataHandler Instance => lazy.Value;
        private Configuration _config;
        public static string CONFIG_FILE => "config.cfg";
        
        public DataHandler()
        {

        }
        public void LoadData()
        {
            string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string configFile = Path.Combine(assemblyPath, CONFIG_FILE);
            if (File.Exists(configFile))
                _config = JsonUtils.Deserialize<Configuration>(configFile);
            else
            {
                _config = Configuration.GenerateConfigFile(configFile);
            }
        }

        public void SaveData()
        {

        }
    }
}
