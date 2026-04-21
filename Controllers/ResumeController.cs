using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NuGet.Common;
using ResumeGenerator.Models;
using ResumeGenerator.Services;

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

        public async Task<IActionResult> Home(string token)
        {

            var profile = await _githubServices.GetProfileAsync(token);
            var repositories = await _githubServices.GetRepositoriesAsync(token);

            var viewModel = new ResumeVM
            {
                Profile = profile,
                Repositories = repositories
            };

            System.Diagnostics.Debug.WriteLine($"Profile: {profile.Name}, amount of repos: {repositories.Count}");
            return View(viewModel);
        }

        public async Task<int> Test()
        {

            await _geminiServices.Test();
            return 1;

        }
    }
}
