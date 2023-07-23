using System;
using System.IO;

namespace ExpenseTracker.Utils
{
    public static class PathUtils
    {
        public static string AppDataPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ExpenseTracker");
        }
    }
}
