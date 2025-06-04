using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace gitChat.Services
{
    public class OllamaService
    {
        private readonly HttpClient _httpClient;

        public OllamaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private async Task<string> SendPromptAsync(string prompt, string systemMessage = "")
        {
            var requestBody = new
            {
                model = "llama3",
                prompt = !string.IsNullOrWhiteSpace(systemMessage) ? $"{systemMessage}\n{prompt}" : prompt,
                stream = false
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://localhost:11434/api/generate", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(responseJson);
            return document.RootElement.GetProperty("response").GetString();
        }

        public Task<string> ChatAsync(string message) => SendPromptAsync(message);

        public Task<string> ExplainTermAsync(string term) =>
            SendPromptAsync($"Explique brevemente o que é \"{term}\" em até 100 palavras.");

        public Task<string> SentimentAnalysisAsync(string text) =>
            SendPromptAsync($"Analise o sentimento da frase abaixo e responda apenas com \"positiva\", \"negativa\" ou \"neutra\":\n{text}");

        public Task<string> SummarizeTextAsync(string text) =>
            SendPromptAsync($"Resuma o texto a seguir em no máximo 100 palavras:\n{text}");
    }
}
