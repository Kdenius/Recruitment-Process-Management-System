using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Repository
{
    public interface ICandidateApplicationRepository
    {
        Task<CandidateApplication> CreateApplication(CandidateApplication candidateApplication);
        Task<List<CandidateApplication>> GetApplicationsByCandidateIdAsync(int candidateId);
        Task<CandidateApplication> GetApplicationsByIdAsync(int applicationId);
        Task<List<object>> GetAllCandidateApplicationsAsync();
        Task<bool> UpdateApplicationStatusAsync(int applicationId, string status, string? details = null);
    }
    public class CandidateApplicationRepository : ICandidateApplicationRepository
    {
        private readonly AppDbContext _con;

        public CandidateApplicationRepository(AppDbContext con)
        {
            _con = con;
        }

        public async Task<CandidateApplication> CreateApplication(CandidateApplication candidateApplication)
        {
            _con.CandidateApplications.AddAsync(candidateApplication);
            await _con.SaveChangesAsync();
            return candidateApplication;
        }

        public async Task<CandidateApplication> GetApplicationsByIdAsync(int applicationId)
        {
            var app =  await _con.CandidateApplications
                .Include(ca => ca.Candidate)
                .Include(ca => ca.Position)
                .Where(ca => ca.ApplicationId == applicationId).FirstOrDefaultAsync();
            return app;
        }

        public async Task<List<CandidateApplication>> GetApplicationsByCandidateIdAsync(int candidateId)
        {
            return await _con.CandidateApplications
                .Where(ca => ca.CandidateId == candidateId)
                .Include(ca => ca.Position)
                .Include(ca => ca.InterviewRounds)
                .Include(ca => ca.CandidateDocuments)
                .OrderByDescending(ca => ca.ApplicationId)
                .ToListAsync();
        }

        public async Task<List<object>> GetAllCandidateApplicationsAsync()
        {
            return await _con.CandidateApplications
                .Include(ca => ca.Candidate)
                    .ThenInclude(c => c.CandidateSkills)
                        .ThenInclude(cs => cs.Skill)
                .Include(ca => ca.Position)
                    .ThenInclude(p => p.PositionSkills)
                        .ThenInclude(ps => ps.Skill)
                .Select(ca => new
                {
                    ApplicationId = ca.ApplicationId,
                    Status = ca.Status,
                    Details = ca.Details,
                    CreatedAt = ca.CreatedAt,

                    Candidate = new
                    {
                        ca.Candidate.CandidateId,
                        ca.Candidate.Name,
                        ca.Candidate.Email,
                        ca.Candidate.PhoneNumber,
                        ca.Candidate.ResumeUrl,

                        Skills = ca.Candidate.CandidateSkills.Select(cs => new
                        {
                            cs.SkillId,
                            cs.Skill.SkillName,
                            cs.YearsOfExperience,
                            cs.IsVerified
                        })
                    },

                    Position = new
                    {
                        ca.Position.PositionId,
                        ca.Position.Title,
                        ca.Position.Status,
                        ca.Position.Location,
                        ca.Position.Type,
                        ca.Position.BaseSalary,
                        ca.Position.MaxSalary,

                        Skills = ca.Position.PositionSkills.Select(ps => new
                        {
                            ps.SkillId,
                            ps.Skill.SkillName,
                            ps.IsRequired
                        })
                    }
                })
                .OrderByDescending(x => x.ApplicationId)
                .ToListAsync<object>();
        }

        public async Task<bool> UpdateApplicationStatusAsync(int applicationId, string status, string? details = null)
        {
            var application = await _con.CandidateApplications.FirstOrDefaultAsync(ca => ca.ApplicationId == applicationId);
            if (application == null)
                return false;

            application.Status = status;

            if (!string.IsNullOrWhiteSpace(details))
                application.Details = details;

            await _con.SaveChangesAsync();
            return true;
        }


    }
}
