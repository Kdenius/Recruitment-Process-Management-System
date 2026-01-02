using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApi.DTO;
using WebApi.Models;
using WebApi.Services;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace WebApi.Repository
{
    public interface ICandidateRepository
    {
        Task<Candidate> CreateCandidateAsync(Candidate candidate);
        Task<IEnumerable<Candidate>> GetAllCandidatesAsync();

    }
    public class CandidateRepository : ICandidateRepository
    {
        private readonly AppDbContext _con;
        private readonly PasswordHasher<Candidate> _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IConfiguration _config;

        public CandidateRepository(IRoleRepository roleRepository, AppDbContext con, IJwtTokenService jwtTokenService, IConfiguration config)
        {
            _con = con;
            _jwtTokenService = jwtTokenService;
            _config = config;
        }
        public async Task<Candidate> CreateCandidateAsync(Candidate candidate)
        {
            _con.Candidates.Add(candidate);
            _con.SaveChanges();
            return candidate;
        }

        public async Task<IEnumerable<Candidate>> GetAllCandidatesAsync()
        {
            return await _con.Candidates.ToListAsync();
        }

    }
}
