using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Repository
{
    public interface IPositionRepository
    {
        Task<Position> CreatePositionAsync(Position position);
        Task<Position> GetPositionByIdAsync(int positionId);
        Task<IEnumerable<Position>> GetAllPositionsAsync();
    }
    public class PositionRepository : IPositionRepository
    {
        private readonly AppDbContext _con;

        public PositionRepository(AppDbContext context)
        {
            _con = context;
        }

        public async Task<Position> CreatePositionAsync(Position position)
        {
            _con.Positions.Add(position);
            await _con.SaveChangesAsync();
            return position;
        }

        public async Task<Position> GetPositionByIdAsync(int positionId)
        {
            return await _con.Positions
                .Include(p => p.PositionSkills)
                .ThenInclude(ps => ps.Skill)
                .FirstOrDefaultAsync(p => p.PositionId == positionId);
        }

        public async Task<IEnumerable<Position>> GetAllPositionsAsync()
        {
            return await _con.Positions.ToListAsync();
        }
    }
}
