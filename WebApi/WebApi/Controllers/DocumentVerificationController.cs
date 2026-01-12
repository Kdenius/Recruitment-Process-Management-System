using Microsoft.AspNetCore.Mvc;
using System;
using WebApi.DTO;
using WebApi.Repository;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/document-verification")]
    public class DocumentVerificationController : ControllerBase
    {
        private readonly IDocumentVerificationRepository _repo;
        private readonly ICandidateApplicationRepository _candidateApplicationRepository;
        private readonly IEmailService _email;

        public DocumentVerificationController(IDocumentVerificationRepository repo, ICandidateApplicationRepository candidateApplicationRepository, IEmailService email)
        {
            _repo = repo;
            _candidateApplicationRepository = candidateApplicationRepository;
            _email = email;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestVerification(DocumentVerificationRequestDTO dto)
        {
            await _repo.SendForDocumentVerificationAsync(dto.ApplicationId, dto.RequiredDocTypeIds);
            var app = await _candidateApplicationRepository.GetApplicationsByIdAsync(dto.ApplicationId);

            await _email.DocumentVerificationEmail(app.Candidate.Name, app.Candidate.Email, app.Position.Title, "NA");

            return Ok(new { message = "Document verification requested" });
        }

        // Candidate → Upload document
        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument([FromForm] UploadDocumentDTO dto)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}_{dto.File.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            await _repo.UploadDocumentAsync(dto.ApplicationId, dto.DocTypeId, $"Uploads/{fileName}");

            return Ok(new { message = "Document uploaded successfully" });
        }

        // HR → View documents
        [HttpGet("{applicationId}")]
        public async Task<IActionResult> GetDocuments(int applicationId)
        {
            var docs = await _repo.GetDocumentsByApplicationAsync(applicationId);
            return Ok(docs);
        }

        // HR → Verify document
        [HttpPatch("verify")]
        public async Task<IActionResult> VerifyDocument(VerifyDocumentDTO dto)
        {
            await _repo.VerifyDocumentAsync(dto.DocumentId);
            return Ok(new { message = "Document verified" });
        }
    }

}
