namespace Documents_OCR_back.Models.DTOs
{
    public class DocumentUploadRequest
    {
        public string FileName { get; set; }
        public IFormFile File { get; set; }
    }
}
