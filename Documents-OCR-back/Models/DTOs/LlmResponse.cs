namespace Documents_OCR_back.Models.DTOs
{
    public class LlmResponse
    {
        public string CorrectedText { get; set; }
        public Dictionary<string, List<WordSuggestion>> Suggestions { get; set; }
    }
}
