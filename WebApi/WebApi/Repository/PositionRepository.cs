using Microsoft.EntityFrameworkCore;
using WebApi.DTO;
using WebApi.Models;

namespace WebApi.Repository
{
    public interface IPositionRepository
    {
        Task<Position> CreatePositionAsync(Position position);
        Task<Position> GetPositionByIdAsync(int positionId);
        Task<IEnumerable<object>> GetAllPositionsAsync();
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

        public async Task<IEnumerable<object>> GetAllPositionsAsync()
        {
            var positions = await _con.Positions
                .Include(p => p.PositionSkills)
                    .ThenInclude(ps => ps.Skill)  
                .Select(p => new 
                {
                    PositionId = p.PositionId,
                    Title = p.Title,
                    Description = p.Description,
                    Status = p.Status,
                    CloserReason = p.CloserReason,
                    Rounds = p.Rounds,
                    CreatedAt = p.CreatedAt,
                    RecruiterId = p.RecruiterId,
                    RecruiterName = p.Recruiter.FirstName,
                    Location = p.Location,
                    Type = p.Type,
                    BaseSalary = p.BaseSalary,
                    MaxSalary = p.MaxSalary,
                    PositionSkills = p.PositionSkills.Select(ps => new 
                    {
                        RoleId = ps.SkillId, // SkillId is the RoleId
                        RoleName = ps.Skill.SkillName,// SkillName is the RoleName
                        IsRequired =  ps.IsRequired
                    }).ToList()
                })
                .ToListAsync();

            return positions;


        }
    }
}
