using Octokit;

namespace ApplicationUpdater
{
    public class AppClientUpdater
    {
        private GitHubClient client;
        private string _gitHubOwner;
        private string _gitHubProject;

        public AppClientUpdater() 
        {

        }

        public void InitializeClient(string clientName, string owner, string project)
        {
            client = new GitHubClient(new ProductHeaderValue(clientName));
            _gitHubOwner = owner;
            _gitHubProject = project;
        }
        public async void CheckForUpdate(string curassemblyVersion = "")
        {
            IReadOnlyList<Release> releases = await client.Repository.Release.GetAll(_gitHubOwner, _gitHubProject);
            var latest = releases[0];
            Console.WriteLine(
                "The latest release is tagged at {0} and is named {1}",
                latest.TagName,
                latest.Name);
        }
    }
}