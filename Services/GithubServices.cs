using Octokit;
using System.Linq;
using ResumeGenerator.Models;
namespace ResumeGenerator.Services
{
    public class GithubServices
    {
        public async Task<GithubProfile> GetProfileAsync(string token)
        {
            var client = CreateClient(token);
            var user = await client.User.Current();

            return new GithubProfile
            {
                Username = user.Login,
                Name = user.Name,
                Biography = user.Bio,
                AvatarUrl = user.AvatarUrl,
                Repos = user.PublicRepos,
                Followers = user.Followers
            };
        }

        public async Task<List<GitHubRepo>> GetRepositoriesAsync(string token)
        {
            var client = CreateClient(token);
            var repositories = await client.Repository.GetAllForCurrent();
            return repositories.Select(repo => new GitHubRepo
            {
                Name = repo.Name,
                Language = repo.Language,
                Stars = repo.StargazersCount,
                Description = repo.Description
            }).ToList();
        }

        // Create and configure an Octokit GitHubClient, adding token credentials when provided
        private GitHubClient CreateClient(string token)
        {
            var client = new GitHubClient(new ProductHeaderValue("ResumeGenerator"));
            if (!string.IsNullOrWhiteSpace(token))
            {
                client.Credentials = new Credentials(token);
            }

            return client;
        }

        
    }
}