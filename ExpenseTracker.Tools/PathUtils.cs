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
            return Path.Combine(AppDataPath(), subFolder);
        }
        public static string AppDataPath(string[] subFolders)
        {
            string subFolderStr = "";
            for(uint i = 0; i < subFolders.Length; ++i)
            {
                subFolderStr += $"{subFolders[i]}\\";
            }
            return Path.Combine(AppDataPath(), subFolderStr);
        }
    }
}
