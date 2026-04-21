using System.Threading.Tasks;
using Google.GenAI;
using Google.GenAI.Types;
using ResumeGenerator.Models;
namespace ResumeGenerator.Services
{

    public class GeminiServices
    {
        private readonly Client _client;
        private readonly string _apiKey;
        public GeminiServices(IConfiguration config)
        {
            
            _apiKey = config["Gemini:ApiKey"]!;
            _client = new Client(apiKey:_apiKey);

        }
        public async Task<string> GenerateResuméAsync(GithubProfile profile, List<GitHubRepo> repositories)
        {

            var prompt = $@"
                Du är en expert på karriärrådgivning vid Harvard Business School. 
                Skapa ett professionellt och stilrent CV i Markdown-format för följande person:

                NAMN: {profile.Name}
                BIO: {profile.Biography}
                PROJEKT (från GitHub): {string.Join(", ", repositories.Select(r => $"{r.Name} ({r.Language}): {r.Description} - Stars: {r.Stars}"))}

                Instruktioner för CV-format:
                1. Använd Harvard-standarden: Stilrent, professionellt och fokus på resultat.
                2. Struktur: Kontaktinformation, Sammanfattning (Professional Summary), Tekniska färdigheter, Projekt (Technical Experience) och Utbildning.
                3. För varje projekt: Skriv 2-3 punkter (bullet points) som beskriver vad projektet gör, vilka tekniker som användes och dess genomslag (använd stjärnorna som bevis på popularitet).
                4. Ton: Formell, handlingsorienterad (använd ord som 'Developed', 'Architected', 'Implemented').
                5. Språk: Engelska (eftersom det är standard för mjukvaruutvecklare).

                Svara endast med CV-innehållet i Markdown.";

            var response = await _client.Models.GenerateContentAsync(
                model: "gemini-3-flash", // Preview-versionen fungerar, men den stabila är ofta snabbare
                contents: prompt
            );
        }

        public async Task Test()
        {

            var response = await _client.Models.GenerateContentAsync(
                model: "gemini-3-flash-preview", contents: "Explain how AI works in a few words"
                );
            System.Diagnostics.Debug.WriteLine(response.Candidates[0].Content.Parts[0].Text);

        }
    }
}
