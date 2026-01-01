using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Repository
{
    public interface ISkillRepository
    {
        Task<Skill> CreateSkillAsync(string skill);
        Task<Skill> GetSkillByIdAsync(int skillId);
        Task<IEnumerable<Skill>> GetSkillsByIdsAsync(List<int> skillIds);
        Task<IEnumerable<Skill>> GetAllSkillsAsync();
    }
    public class SkillRepository : ISkillRepository
    {
        private readonly AppDbContext _con;

        public SkillRepository(AppDbContext db)
        {
            _con = db;
        }
        public async Task<Skill> CreateSkillAsync(string skill)
        {
            var obj = new Skill()
            {
                SkillName = skill,
            };
            _con.Skills.Add(obj);
            await _con.SaveChangesAsync();
            return obj;
        }
        public async Task<Skill> GetSkillByIdAsync(int skillId)
        {
            return await _con.Skills
                .FirstOrDefaultAsync(s => s.SkillId == skillId);
        }
        public async Task<IEnumerable<Skill>> GetSkillsByIdsAsync(List<int> skillIds)
        {
            return await _con.Skills.Where(s => skillIds.Contains(s.SkillId)).ToListAsync();
        }

        public async Task<IEnumerable<Skill>> GetAllSkillsAsync()
        {
            return await _con.Skills.ToListAsync();
        }
    }
}
