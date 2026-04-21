namespace ResumeGenerator.Models
{
    public class ResumeVM
    {

        public GithubProfile Profile { get; set; }
        public List<GitHubRepo> Repositories { get; set; }
    }
}
