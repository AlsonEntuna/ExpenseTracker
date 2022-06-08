using ExpenseTracker.Utils;
using System;
using System.IO;
using System.Reflection;

namespace ExpenseTracker.Data
{

    public static class DataHandler
    {
        public static Configuration Config;
        public static string CONFIG_FILE => "config.cfg";

        public static void LoadAppConfiguration()
        {
            string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string configFile = Path.Combine(assemblyPath, CONFIG_FILE);
            if (File.Exists(configFile))
                Config = JsonUtils.Deserialize<Configuration>(configFile);
            else
            {
                Config = Configuration.GenerateConfigFile(configFile);
            }
        }
    }
}
