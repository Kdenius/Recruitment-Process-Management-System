using Microsoft.EntityFrameworkCore;
using WebApi.Models;


namespace WebApi.Repository
{

    public interface IDocumentTypeRepository
    {
        Task<IEnumerable<DocumentType>> GetAllAsync();
        Task<DocumentType> CreateAsync(DocumentType documentType);
    }
    public class DocumentTypeRepository : IDocumentTypeRepository
    {
        private readonly AppDbContext _context;

        public DocumentTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DocumentType>> GetAllAsync()
        {
            return await _context.DocumentTypes
                .OrderBy(d => d.DocTypeName)
                .ToListAsync();
        }

        public async Task<DocumentType> CreateAsync(DocumentType documentType)
        {
            _context.DocumentTypes.Add(documentType);
            await _context.SaveChangesAsync();
            return documentType;
        }
    }
}
