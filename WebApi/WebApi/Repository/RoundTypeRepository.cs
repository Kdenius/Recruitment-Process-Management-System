using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Repository
{
    public interface IRoundTypeRepository
    {
        Task<IEnumerable<RoundType>> GetAllAsync();
        Task<RoundType> CreateAsync(RoundType roundType);
    }

    public class RoundTypeRepository : IRoundTypeRepository
    {
        private readonly AppDbContext _context;

        public RoundTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoundType>> GetAllAsync()
        {
            return await _context.RoundTypes.ToListAsync();
        }

        public async Task<RoundType> CreateAsync(RoundType roundType)
        {
            _context.RoundTypes.Add(roundType);
            await _context.SaveChangesAsync();
            return roundType;
        }
    }
}
