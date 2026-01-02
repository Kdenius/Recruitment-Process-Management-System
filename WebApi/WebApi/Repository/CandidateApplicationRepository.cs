using WebApi.Models;

namespace WebApi.Repository
{
    public interface ICandidateApplicationRepository
    {
        Task<CandidateApplication> CreateApplication(CandidateApplication candidateApplication);
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
    }
}
