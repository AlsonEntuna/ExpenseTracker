namespace ExpenseTracker.Tools
{
    public static class PathUtils
    {
        public static string AppDataPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }
        public static string AppDataPath(string subFolder, bool createIfNotExist = true)
        {
            string appDataPath = Path.Combine(AppDataPath(), subFolder);
            if (createIfNotExist)
                Directory.CreateDirectory(appDataPath);
            return appDataPath;
        }
        public static string AppDataPath(string[] subFolders, bool createIfNotExist = true)
        {
            string subFolderStr = "";
            for(uint i = 0; i < subFolders.Length; ++i)
            {
                subFolderStr += $"{subFolders[i]}\\";
            }
            string path = Path.Combine(AppDataPath(), subFolderStr);
            if (createIfNotExist)
                Directory.CreateDirectory(path);
            return path;
        }
    }
}
