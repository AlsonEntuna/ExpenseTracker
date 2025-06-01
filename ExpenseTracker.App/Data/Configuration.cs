using ExpenseTracker.Tools;

using System;
using System.Collections.Generic;

namespace ExpenseTracker.Data
{
    [Serializable]
    public class Configuration
    {
        // TODO: we need to change the data location from string to List<string> to contain more than 1 string
        //public string DataLocation { get; set; }
        public List<string> DataLocations { get; set; } = new List<string>();
        public Configuration() { }

        public static Configuration GenerateConfigFile(string path)
        {
            Configuration config = new();
            JsonUtils.Serialize(path, config);
            return config;
        }
    }
}
