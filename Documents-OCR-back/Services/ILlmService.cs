namespace Documents_OCR_back.Services
{
    public interface ILlmService
    {
        Task<(string CorrectedText, Dictionary<string, List<string>> Suggestions)> CorrectTextAsync(string textExtracted);
    }
}
