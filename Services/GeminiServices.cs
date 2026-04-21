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
                3. För varje projekt: Skriv 2-3 punkter (bullet points) som beskriver vad projektet gör, vilka tekniker som användes och dess genomslag.
                4. Ton: Formell, handlingsorienterad (använd ord som 'Developed', 'Architected', 'Implemented').
                5. Språk: Engelska (själva CV:t ska vara på engelska).§

                Obs: Det 'språk' som anges är bara GitHubs automatiska gissning baserat på dominerande filtyp. Använd istället projektets namn och beskrivning
                för att lista ut den faktiska tekniska stacken. Om det till exempel står 'CSS' men beskrivningen nämner 
                'components', 'hooks' eller 'state management',så är det förmodligen ett React/JavaScript-projekt.

                Svara endast med CV-innehållet i Markdown.";

         

            try
            {
                var response = await _client.Models.GenerateContentAsync(
                    model: "gemini-3-flash",
                    contents: prompt
                );

                // Hämta texten från det första svaret (Candidate)
                // I det nya SDK:t kan du oftast skriva response.Text, 
                // men här är den säkra vägen genom objekthierarkin:
                var generatedText = response.Candidates[0].Content.Parts[0].Text;

                return generatedText ?? "Kunde inte generera innehåll.";
            }
            catch (Exception ex)
            {
                // Logga felet (viktigt för API-anrop!)
                System.Diagnostics.Debug.WriteLine($"Gemini Error: {ex.Message}");
                return "Ett fel uppstod vid generering av ditt CV. Försök igen senare.";
            }
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
