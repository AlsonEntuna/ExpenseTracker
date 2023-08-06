namespace ExpenseTracker.Tools
{
    public static class PathUtils
    {
        public static string AppDataPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }
        public static string AppDataPath(string subFolder) 
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), subFolder);
        }
    }
}
