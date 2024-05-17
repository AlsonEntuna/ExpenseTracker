namespace ApplicationUpdater
{
    public static class HttpClientUtils
    {
        public static async Task DownloadFileAsync(this HttpClient client, Uri uri, string targetPath)
        {
            using (Stream stream = await client.GetStreamAsync(uri))
            using (FileStream fs = new FileStream(targetPath, FileMode.CreateNew))
            {
                await stream.CopyToAsync(fs);
            }
        }
    }
}
