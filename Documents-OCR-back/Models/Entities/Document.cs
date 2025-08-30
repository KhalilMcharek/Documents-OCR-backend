namespace Documents_OCR_back.Models.Entities
{
    public class Document
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string TextExtracted { get; set; }
        public string CorrectedText { get; set; }
        public DateTime UploadedAt { get; set; }
        public string SuggestionsJson { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
