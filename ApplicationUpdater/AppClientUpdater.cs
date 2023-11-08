﻿using System.Net;
using Octokit;
using ExpenseTracker.Tools;

namespace ApplicationUpdater
{
    public class DownloadInstallerCompleteArgs : EventArgs
    {
        public string InstallerPath;
        public string Version;
        public DownloadInstallerCompleteArgs(string installerPath, string version) 
        {
            InstallerPath = installerPath; 
            Version = version;
        }
    }

    public class AppClientUpdater
    {
        private GitHubClient _client;
        private string _gitHubOwner;
        private string _gitHubProject;

        string _downloadFilename = Path.Combine(PathUtils.AppDataPath(
            new string[] { "ExpenseTracker", "downloads" }),
            "ExpenseTrackerSetup.exe");

        public EventHandler<DownloadInstallerCompleteArgs> DownloadCompleted;

        public AppClientUpdater() 
        {
        }

        public void InitializeClient(string clientName, string owner, string project)
        {
            _client = new GitHubClient(new ProductHeaderValue(clientName));
            _gitHubOwner = owner;
            _gitHubProject = project;
        }
        public async void CheckForUpdate(string curassemblyVersion)
        {
            IReadOnlyList<Release> releases = await _client.Repository.Release.GetAll(_gitHubOwner, _gitHubProject);
            var latest = releases[0];
            Console.WriteLine(
                "The latest release is tagged at {0} and is named {1}",
                latest.TagName,
                latest.Name);
            // TODO: download if not up-to-date
            // Run the installer
            bool inLatest = string.Equals(latest.TagName, curassemblyVersion);
            if (!inLatest)
            {
                WebClient wclient = new();
                if (File.Exists(_downloadFilename)) 
                {
                    File.Delete(_downloadFilename);
                }
                await wclient.DownloadFileTaskAsync(new Uri(latest.Assets[0].BrowserDownloadUrl), _downloadFilename);
                if (File.Exists(_downloadFilename)) 
                {
                    DownloadCompleted?.Invoke(this, new DownloadInstallerCompleteArgs(_downloadFilename, latest.TagName));
                }
            }
        }
    }
}