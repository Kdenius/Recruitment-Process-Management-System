using Microsoft.AspNetCore.Mvc;
using System;
using WebApi.DTO;
using WebApi.Repository;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("document-verification")]
    public class DocumentVerificationController : ControllerBase
    {
        private readonly IDocumentVerificationRepository _repo;
        private readonly ICandidateApplicationRepository _candidateApplicationRepository;
        private readonly IEmailService _email;
        private readonly IFileUploadService _fileUploadService;

        public DocumentVerificationController(IDocumentVerificationRepository repo, ICandidateApplicationRepository candidateApplicationRepository, IEmailService email, IFileUploadService fileUploadService)
        {
            _repo = repo;
            _candidateApplicationRepository = candidateApplicationRepository;
            _email = email;
            _fileUploadService = fileUploadService;
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
            var existingDoc = await _repo.GetDocumentAsync(
                dto.ApplicationId,
                dto.DocTypeId
            );
            if (existingDoc != null)
            {
                _fileUploadService.DeleteFile(existingDoc.DocumentUrl);
            }
            var filePath = await _fileUploadService.UploadFileAsync(dto.File);

            await _repo.UploadDocumentAsync(dto.ApplicationId, dto.DocTypeId, filePath);

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

        // Candidate ne doc jova
        [HttpGet("candidate/{candidateId}")]
        public async Task<IActionResult> GetDocumentsByCandidate(int candidateId)
        {
            var docs = await _repo.GetDocumentsByCandidateForVerificationAsync(candidateId);
            return Ok(docs);
        }

    }

}
