namespace Documents_OCR_back.Services
{
    public interface IOcrService
    {
        Task<string> ExtractText(string filePath);
    }
}
