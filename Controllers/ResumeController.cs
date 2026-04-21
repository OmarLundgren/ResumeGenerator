using Microsoft.AspNetCore.Authentication;

using Microsoft.AspNetCore.Mvc;

using ResumeGenerator.Models;
using ResumeGenerator.Services;
using Markdig;

namespace ResumeGenerator.Controllers
{
    public class ResumeController : Controller
    {
        private readonly GithubServices _githubServices;
        private readonly GeminiServices _geminiServices;
    
        public ResumeController(GithubServices githubServices, GeminiServices geminiServices)
        {
            _githubServices = githubServices;
            _geminiServices = geminiServices;
        }

        public async Task<IActionResult> Index()
        {

            return View();

        }

        public async Task<IActionResult> Home()
        {
            var token = HttpContext.Session.GetString("GitHubToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index");
            }

            var profile = await _githubServices.GetProfileAsync(token);
            var repositories = await _githubServices.GetRepositoriesAsync(token);

            var viewModel = new ProfileVM
            {
                Profile = profile,
                Repositories = repositories
            };

            System.Diagnostics.Debug.WriteLine($"Profile: {profile.Name}, amount of repos: {repositories.Count}");
            return View(viewModel);
        }

        public async Task<IActionResult> Resume()
        {
            var token = HttpContext.Session.GetString("GitHubToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index");
            }

            var profile = await _githubServices.GetProfileAsync(token);
            var repositories = await _githubServices.GetRepositoriesAsync(token);

            var markdownResume = await _geminiServices.GenerateResuméAsync(profile, repositories);
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var resumeHtml = Markdown.ToHtml(markdownResume, pipeline);

            var resumeModel = new ResumeModel
            {
                HtmlContent = resumeHtml
            };
            return View(resumeModel);
        }

    }
}
