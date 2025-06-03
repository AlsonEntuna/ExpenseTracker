using ExpenseTracker.Tools;

using System;
using System.Collections.Generic;

namespace ExpenseTracker.Data
{
    [Serializable]
    public class Configuration
    {
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
