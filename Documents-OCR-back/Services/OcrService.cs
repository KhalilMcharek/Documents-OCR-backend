using System.Net;
using Documents_OCR_back.Services;

namespace Documents_OCR_back.Services
{
    public class OcrService : IOcrService
    { private readonly HttpClient _httpClient;

        public OcrService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> ExtractText(string filePath)
        {
            using var form = new MultipartFormDataContent();
            using var fileStream = File.OpenRead(filePath);
            form.Add(new StreamContent(fileStream), "file", Path.GetFileName(filePath));

            var response = await _httpClient.PostAsync("/api/ocr/extract", form);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Erreur lors de l'appel OCR");

            return await response.Content.ReadAsStringAsync();
        }

    }
}
