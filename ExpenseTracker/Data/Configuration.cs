using ExpenseTracker.ExpenseSys;
using ExpenseTracker.Utils;
using System;
using System.IO;

namespace ExpenseTracker.Data
{
    [Serializable]
    public class Configuration
    {
        public string DataLocation { get; set; }
        public Configuration() { }

        public static Configuration GenerateConfigFile(string path)
        {
            Configuration config = new Configuration();
            // TODO: Generate the default path
            config.DataLocation = Path.Combine(path, "Data", Constants.DEFAULT_DATA_FILENAME);
            JsonUtils.Serialize(path, config);
            return config;
        }
    }
}
