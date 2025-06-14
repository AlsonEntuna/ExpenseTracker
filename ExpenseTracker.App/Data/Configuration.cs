using ExpenseTracker.Tools;

using System;
using System.Collections.Generic;

namespace ExpenseTracker.Data
{
    [Serializable]
    public class Configuration
    {
        private List<string> _dataLocations = new List<string>();
        public IReadOnlyList<string> DataLocations => _dataLocations;
        public Configuration() { }

        public static Configuration GenerateConfigFile(string path)
        {
            Configuration config = new();
            JsonUtils.Serialize(path, config);
            return config;
        }

        public void AddDataLocationEntry(string entry)
        {
            if (_dataLocations.Contains(entry))
            {
                return;
            }
            _dataLocations.Add(entry);
        }

        public void RemoveDataLocationEntry(string entry)
        {
            if (!_dataLocations.Contains(entry))
            {
                return;
            }
            _dataLocations.Remove(entry);
        }
    }
}
