using Documents_OCR_back.Models.DTOs;
using System.Text.Json;

namespace Documents_OCR_back.Services
{
    public class LlmService : ILlmService
    {
        private readonly HttpClient _httpClient;

        public LlmService(HttpClient httpClient)
        {
            _httpClient = httpClient;
           
        }

        public async Task<(string CorrectedText, Dictionary<string, List<string>> Suggestions)> CorrectTextAsync(string textExtracted)
        {
            var request = new { text = textExtracted };
            var response = await _httpClient.PostAsJsonAsync("api/llm/correct", request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"LLM service failed: {response.StatusCode} - {error}");
            }

            var rawJson = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Réponse brute LLM : " + rawJson);

            var result = JsonSerializer.Deserialize<LlmResponse>(rawJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result == null)
                throw new Exception("Erreur de désérialisation : réponse LLM vide ou incorrecte.");

            if (string.IsNullOrWhiteSpace(result.CorrectedText))
                result.CorrectedText = "(aucune correction)";

            var suggestions = new Dictionary<string, List<string>>();
            if (result.Suggestions != null)
            {
                foreach (var kvp in result.Suggestions)
                {
                    suggestions[kvp.Key] = kvp.Value?.Select(w => w.Word).ToList() ?? new List<string>();
                }
            }

            return (result.CorrectedText, suggestions);
        }

    }
}
