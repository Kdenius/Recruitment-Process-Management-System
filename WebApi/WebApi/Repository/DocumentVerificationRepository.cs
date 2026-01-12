using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Repository
{
    public interface IDocumentVerificationRepository
    {
        Task SendForDocumentVerificationAsync(int applicationId, List<int> docTypeIds);
        Task UploadDocumentAsync(int applicationId, int docTypeId, string filePath);
        Task<List<CandidateDocument>> GetDocumentsByApplicationAsync(int applicationId);
        Task VerifyDocumentAsync(int documentId);
    }
    public class DocumentVerificationRepository : IDocumentVerificationRepository
    {
        private readonly AppDbContext _context;

        public DocumentVerificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task SendForDocumentVerificationAsync(int applicationId, List<int> docTypeIds)
        {
            var application = await _context.CandidateApplications
                .Include(a => a.CandidateDocuments)
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

            if (application == null)
                throw new Exception("Application not found");

            application.Status = "DocVerification";

            foreach (var docTypeId in docTypeIds)
            {
                if (!application.CandidateDocuments.Any(d => d.DocTypeId == docTypeId))
                {
                    application.CandidateDocuments.Add(new CandidateDocument
                    {
                        ApplicationId = applicationId,
                        DocTypeId = docTypeId,
                        IsVerified = false
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task UploadDocumentAsync(int applicationId, int docTypeId, string filePath)
        {
            var document = await _context.CandidateDocuments
                .FirstOrDefaultAsync(d =>
                    d.ApplicationId == applicationId &&
                    d.DocTypeId == docTypeId);

            if (document == null)
                throw new Exception("Document request not found");

            document.DocumentUrl = filePath;
            document.UploadedAt = DateTime.UtcNow;

            var allUploaded = await _context.CandidateDocuments
                .Where(d => d.ApplicationId == applicationId)
                .AllAsync(d => d.DocumentUrl != null);

            if (allUploaded)
            {
                var app = await _context.CandidateApplications.FindAsync(applicationId);
                app.Status = "DocsSubmitted";
            }

            await _context.SaveChangesAsync();
        }

        // HR views 
        public async Task<List<CandidateDocument>> GetDocumentsByApplicationAsync(int applicationId)
        {
            return await _context.CandidateDocuments
                .Include(d => d.DocumentType)
                .Where(d => d.ApplicationId == applicationId)
                .ToListAsync();
        }

        // HR verifies 
        public async Task VerifyDocumentAsync(int documentId)
        {
            var doc = await _context.CandidateDocuments.FindAsync(documentId);

            if (doc == null)
                throw new Exception("Document not found");

            doc.IsVerified = true;

            var allVerified = await _context.CandidateDocuments
                .Where(d => d.ApplicationId == doc.ApplicationId)
                .AllAsync(d => d.IsVerified);

            if (allVerified)
            {
                var app = await _context.CandidateApplications.FindAsync(doc.ApplicationId);
                app.Status = "DocsVerified";
            }

            await _context.SaveChangesAsync();
        }
    }

}
