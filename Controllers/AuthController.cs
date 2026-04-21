using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ResumeGenerator.Models;
using ResumeGenerator.Services;

namespace ResumeGenerator.Controllers
{

    public class AuthController : Controller
    {
       
    

    
        private readonly IConfiguration _config;
        public AuthController(IConfiguration config)
        {
            _config = config;
        }
        [HttpGet]
        public IActionResult Login()
        {
            
            var state = Guid.NewGuid().ToString();
            HttpContext.Session.SetString("OAuthState", state);


            var clientId = _config["GitHub:ClientId"];
            var redirectUri = "https://localhost:7104/Auth/Callback";
            var scope = "repo user";

            var url = $"https://github.com/login/oauth/authorize?client_id={clientId}&redirect_uri={redirectUri}&scope={scope}&state={state}";
            return Redirect(url);

        }
        public async Task<IActionResult> Callback(string state, string code)
        {
            System.Diagnostics.Debug.WriteLine($"State: {state}, Code: {code}");
            var savedState = HttpContext.Session.GetString("OAuthState");

            if(string.IsNullOrEmpty(savedState) || savedState != state) {
                return BadRequest("Invalid state");
            }

            HttpContext.Session.Remove("OAuthState");

            var token = await ExchangeCodeForToken(code);
            if (token == null) { 
                return BadRequest("Failed to retrieve access token");
            }

            return RedirectToAction("Home", "Resume", new {token = token});
            

        }
        /*
         *Används för att byta ut den temporära koden som GitHub skickar 
         *tillbaka mot en access token som kan användas för att hämta användarens data.
         *
         */
        private async Task<string?> ExchangeCodeForToken(string code)
        {
            //spara när värdena för client_id, client_secret, code och redirect_uri i en dictionary,
            //som ska skickas med till GitHub i en POST request
            var values = new Dictionary<string, string>
            {
                { "client_id", _config["GitHub:ClientId"] },
                { "client_secret", _config["GitHub:ClientSecret"] },
                { "code", code },
                { "redirect_uri", "https://localhost:7104/Auth/Callback" } //extra sälerhetsåtgärd, menas INTE att json ska skickas till auth/callback
            };
            var content = new FormUrlEncodedContent(values);

            // Skicka ett dolt POST-anrop till GitHub
            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync("https://github.com/login/oauth/access_token", content);

            if (response.IsSuccessStatusCode) { 

                var result = await response.Content.ReadAsStringAsync();
                var resultstring = System.Web.HttpUtility.ParseQueryString(result);

                return resultstring["access_token"];
            }
            return null;

        }
        public async Task<IActionResult> Logout()
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Resume");
        }

    }
}
