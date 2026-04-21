namespace ResumeGenerator.Models
{
    public class ProfileVM
    {

        public GithubProfile Profile { get; set; }
        public List<GitHubRepo> Repositories { get; set; }
    }
}
