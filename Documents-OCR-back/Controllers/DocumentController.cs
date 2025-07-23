using Documents_OCR_back.Models.DTOs;
using Documents_OCR_back.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Documents_OCR_back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class DocumentController : ControllerBase
    {
        private readonly DocumentService _documentService;
     

        public DocumentController(DocumentService documentService)
        {
            _documentService = documentService;
           
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument([FromForm] IFormFile file, [FromForm] string? documentName)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Aucun fichier envoyé.");

            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            var doc = await _documentService.Upload(file, documentName, userId);
            return Ok(new { message = "Document uploadé", document = doc });
        }





        [HttpPost("extract/{id}")]
        public async Task<IActionResult> ExtractTextFromDocument(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var text = await _documentService.ExtractTextFromDocument(id, userId);
            if (text == null)
                return Forbid();

            return Ok(new { text });
        }


        [HttpGet]
        public async Task<IActionResult> GetDocuments()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var docs = await _documentService.ListeDocument(userId);
            return Ok(docs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocument(int id)
        {
            var doc = await _documentService.DocumentById(id);
            if (doc == null)
                return NotFound(new { message = "Document introuvable" });
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (doc.UserId != userId)
                return Forbid(); 

            return Ok(doc);
        }

        //[Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var doc = await _documentService.DocumentById(id); 
            if (doc == null)
                return NotFound(new { message = "Document introuvable" });

            if (doc.UserId != userId)
                return Forbid(); 

            await _documentService.DeleteDocument(id); 
            return Ok(new { message = "Document supprimé avec succès" });
        }

    }
}
