using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Repository
{
    public interface ICandidateApplicationRepository
    {
        Task<CandidateApplication> CreateApplication(CandidateApplication candidateApplication);
        Task<List<CandidateApplication>> GetApplicationsByCandidateIdAsync(int candidateId);
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
    }
}
