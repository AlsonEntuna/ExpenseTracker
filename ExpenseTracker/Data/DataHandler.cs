﻿using ExpenseTracker.ExpenseSys;
using ExpenseTracker.Utils;
using System;
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
            _categFile = Path.Combine(appDataPath, Constants.CATEGORIES_FILE);
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
                _categFile = Path.Combine(appDataPath, Constants.CATEGORIES_FILE);
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
