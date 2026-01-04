using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
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
        Task<Candidate> GetById(int id);
        Task<ApiResponse<object>> Login(LoginRequest loginRequest);

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

        public async Task<Candidate> GetById(int id)
        {
            return await _con.Candidates.FirstOrDefaultAsync(c => c.CandidateId == id);
        }

        public async Task<ApiResponse<object>> Login(LoginRequest loginRequest)
        {
            var can = await _con.Candidates.FirstOrDefaultAsync(c => c.Email == loginRequest.Email);
            if (can == null)
                return new ApiResponse<object>(success: false, message: $"Candidate not register with {loginRequest.Email}", data: default);
            if (can.PasswordHash != null && can.PasswordHash != loginRequest.Password)
                return new ApiResponse<object>(success: false, message: $"Password mismatch, Please verify credential", data: default);
            var canSkills = await (from cs in _con.CandidateSkills
                                   join s in _con.Skills on cs.SkillId equals s.SkillId
                                   where cs.CandidateId == can.CandidateId
                                   select new
                                   {
                                       SkillId = s.SkillId,
                                       SkillName = s.SkillName,
                                       YearsOfExperience = cs.YearsOfExperience,
                                       IsVerified = cs.IsVerified
                                   }).ToListAsync();
            var res = new 
            {
                CandidateId = can.CandidateId,
                Name = can.Name,
                Email = can.Email,
                RoleName = "Candidate",
                PhoneNumber = can.PhoneNumber,
                CurrentStatus = can.CurrentStatus,
                CreatedAt = can.CreatedAt,
                ResumeUrl = can.ResumeUrl,
                candidateSkills = canSkills
            };
            return new ApiResponse<object>(success: true, message: $"Login Successfully", data: res);
        }

           /* public async Task<ApiResponse<LoginResponseDTO>> Login(LoginRequest loginRequest)
        {
            var user = await _con.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.Email);
            //if user null to fult and message not 
            if (user == null)
                return new ApiResponse<LoginResponseDTO>(success: false, message: $"User does not Exist with {loginRequest.Email}", data: default);
            if (!user.IsVerified)
                return new ApiResponse<LoginResponseDTO>(success: false, message: $"User not Verified! check Verification mail");
            if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginRequest.Password) == PasswordVerificationResult.Failed)
                return new ApiResponse<LoginResponseDTO>(success: false, message: $"Password mismatch", data: default);

            var roleName = _con.Roles.FirstOrDefault(r => r.RoleId == user.RoleId).RoleName;

            user.RefreshToken = _jwtTokenService.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            var response = new LoginResponseDTO()
            {
                UserId = user.UserId,
                Email = loginRequest.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoleId = user.RoleId,
                RoleName = roleName,
                JwtToken = _jwtTokenService.GenerateJwtToken(user.UserId, loginRequest.Email, roleName),
                RefreshToken = user.RefreshToken,
            };
            _con.SaveChanges();

            return new ApiResponse<LoginResponseDTO>(success: true, message: $"Login Successfully", data: response);
        }*/
    }
}
