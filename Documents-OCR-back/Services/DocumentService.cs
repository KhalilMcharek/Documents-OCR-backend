using Documents_OCR_back.Data;
using Documents_OCR_back.Models.Entities;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
namespace Documents_OCR_back.Services

{
    public class DocumentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IOcrService _ocrService;
        private readonly ILlmService _llmService;
        private readonly IConfiguration _configuration;
        public DocumentService(ApplicationDbContext context, IWebHostEnvironment env, ILlmService llmService, IOcrService ocrService, IConfiguration configuration)
        {
            _context = context;
            _env = env;
            _ocrService = ocrService;
            _llmService = llmService;
            _configuration = configuration;
        }



        public async Task<Document> Upload(IFormFile file, string? documentName, int userId)
        {
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsDir);

            var fileExtension = Path.GetExtension(file.FileName);
            var fileName = !string.IsNullOrEmpty(documentName)
                ? $"{Path.GetFileNameWithoutExtension(documentName)}{fileExtension}"
                : $"{Guid.NewGuid()}{fileExtension}";

            var filePath = Path.Combine(uploadsDir, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var doc = new Document
            {
                FileName = fileName,
                TextExtracted = "",
                CorrectedText = "",
                SuggestionsJson = "",
                UserId = userId,
                UploadedAt = DateTime.UtcNow
            };

            _context.Documents.Add(doc);
            await _context.SaveChangesAsync();

            return doc;
        }

        // historique ( liste)
        public async Task<List<Document>> ListeDocument( int userId)
        {
            return await _context.Documents.Where(d => d.UserId == userId).ToListAsync();
        }
         //document par id 
        public async Task <Document> DocumentById ( int id)
        {
            return await _context.Documents.FirstOrDefaultAsync(d => d.Id == id);
        }
        
        //supprimer 
        public async Task DeleteDocument(int id)
        {
            var doc = await _context.Documents.FindAsync(id);
            if (doc == null)
                throw new Exception("Document not found");

            _context.Documents.Remove(doc);
            await _context.SaveChangesAsync();
        }

        public async Task<string?> ExtractTextFromDocument(int documentId, int userId)
        {
            var document = await _context.Documents.FindAsync(documentId);
            if (document == null || document.UserId != userId)
                return null;

            if (string.IsNullOrEmpty(document.FileName))
                throw new Exception("Document file name is null");

            // Utilisation d'un chemin absolu fiable
            var root = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(root, "wwwroot", "uploads", document.FileName);
            Console.WriteLine($"Full filePath: {filePath}");

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Fichier introuvable", filePath);

            var extractedText = await _ocrService.ExtractText(filePath);
            document.TextExtracted = extractedText;

            _context.Documents.Update(document);
            await _context.SaveChangesAsync();

            return extractedText;
        }

        public async Task<(string CorrectedText, Dictionary<string, List<string>> Suggestions)> CorrectTextFromDocument(int documentId, int userId)
        {
            var document = await _context.Documents.FindAsync(documentId);
            if (document == null || document.UserId != userId)
                throw new UnauthorizedAccessException("Document non trouvé ou accès non autorisé.");

            if (string.IsNullOrEmpty(document.TextExtracted))
                throw new ArgumentException("Aucun texte extrait disponible pour correction.");

            var (correctedText, suggestions) = await _llmService.CorrectTextAsync(document.TextExtracted);
            document.CorrectedText = string.IsNullOrWhiteSpace(correctedText) ? "(aucune correction)" : correctedText;
            document.SuggestionsJson = JsonSerializer.Serialize(suggestions ?? new());

            _context.Documents.Update(document);
            await _context.SaveChangesAsync();

            return (correctedText, suggestions);
        }

    }
}
