using ExpenseTracker.Utils;
using System;

namespace ExpenseTracker.Data
{
    [Serializable]
    public class Configuration
    {
        public string DataLocation { get; set; }
        public Configuration() { }

        public static Configuration GenerateConfigFile(string path)
        {
            Configuration config = new()
            {
                DataLocation = ""
            };
            JsonUtils.Serialize(path, config);
            return config;
        }
    }
}
