using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Collections;
using System.Reflection.Metadata.Ecma335;
using WebApi.DTO;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Repository
{
    public interface IUserRepository 
    {
        User AddUser(UserDTO userDTO);
        User GetById(int id);
        User GetByEmail(string email);
        bool VerifyToken(string token, int  userId);
        Task<ApiResponse<LoginResponseDTO>> Login(LoginRequest loginRequest);
        Task<ApiResponse<LoginResponseDTO>> LoginViaRefreshToken(string refreshToken);
        Task<IEnumerable<UserRoleDTO>> GetAll();
        bool UpdateUserRole(int userId, int roleId);

        Task<IEnumerable<UserRoleDTO>> GetInterviewers();
        //string VerifyAdmin(string password);
        ApiResponse<object> VerifyAdmin(string password);
    }
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _con;
        private readonly IRoleRepository _roleRepository;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IConfiguration _config;

        public UserRepository(AppDbContext con, IRoleRepository roleRepository, IJwtTokenService jwtTokenService, IConfiguration config)
        {
            _con = con;
            _roleRepository = roleRepository;
            _passwordHasher = new PasswordHasher<User>();
            _jwtTokenService = jwtTokenService;
            _config = config;
        }

        public User AddUser(UserDTO userDTO)
        {
            var role = _roleRepository.GetRole("Viewer");
            var usr = new User()
            {
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Email = userDTO.Email,
                Role = role,
                CreatedAt = DateTime.UtcNow,
                VerifyToken = TokenGenerator.GenerateRandomizeToken(),
            };
            usr.PasswordHash = _passwordHasher.HashPassword(usr, userDTO.Password);
            _con.Users.Add(usr);
            _con.SaveChanges();
            return usr;
        }
        public User GetById(int id)
        {
            return _con.Users.FirstOrDefault(user => user.UserId == id);
        }
        public User GetByEmail(string email)
        {
            return _con.Users.FirstOrDefault(user => user.Email == email);
        }

        public bool VerifyToken(string token, int userId)
        {
            var user = _con.Users.FirstOrDefault(user => user.UserId == userId);
            if(user.VerifyToken == token)
            {
                user.IsVerified = true;
                _con.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<ApiResponse<LoginResponseDTO>> Login(LoginRequest loginRequest)
        {
            var user = await _con.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.Email);
            //if user null to fult and message not 
            if (user == null)
                return new ApiResponse<LoginResponseDTO>(success: false, message: $"User does not Exist with {loginRequest.Email}", data:default);
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
        }

        public async Task<ApiResponse<LoginResponseDTO>> LoginViaRefreshToken(string refreshToken)
        {
            var user = await _con.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user == null)
                return new ApiResponse<LoginResponseDTO>(success: false, message: "Invalid RefreshToken");
            if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
                return new ApiResponse<LoginResponseDTO>(success: false, message: "Token is Expire");

            var roleName = _con.Roles.FirstOrDefault(r => r.RoleId == user.RoleId).RoleName;
            user.RefreshToken = _jwtTokenService.GenerateRefreshToken();
            var response = new LoginResponseDTO()
            {
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoleId = user.RoleId,
                RoleName = roleName,
                JwtToken = _jwtTokenService.GenerateJwtToken(user.UserId, user.Email, roleName),
                RefreshToken = user.RefreshToken,
            };
            _con.SaveChanges();
            return new ApiResponse<LoginResponseDTO>(success:true, message:"Token Validated", data: response);
        }
        /*public string VerifyAdmin(string password)
        {
            return _passwordHasher.HashPassword(default, password);
        }*/

        public ApiResponse<object> VerifyAdmin(string password)
        {
            var isValid = _passwordHasher.VerifyHashedPassword(default, _config["Admin:PasswordHashed"], password) == PasswordVerificationResult.Success;
            if (!isValid)
                return new ApiResponse<object>(false, "Invalid Password");
            string JwtToken = _jwtTokenService.GenerateJwtToken(0, _config["Admin:Mail"], "Admin");
            return new ApiResponse<object>(true, "Login Successfully", data: JwtToken);
        }

        public async Task<IEnumerable<UserRoleDTO>> GetAll()
        {
            var ret = await (from user in _con.Users
                             join role in _con.Roles on user.RoleId equals role.RoleId
                             select new UserRoleDTO
                             {
                                 UserId = user.UserId,
                                 FirstName = user.FirstName,
                                 LastName = user.LastName,
                                 Email = user.Email,
                                 CreatedAt = user.CreatedAt,
                                 RoleId = role.RoleId,
                                 RoleName = role.RoleName,
                                 IsVerified = user.IsVerified
                             }).ToListAsync<UserRoleDTO>();
            return ret;
        }

        public bool UpdateUserRole(int userId, int roleId)
        {
            var user = GetById(userId);
            if (user == null) return false;
            user.RoleId = roleId;
            _con.SaveChanges();
            return true;
        }

        public async Task<IEnumerable<UserRoleDTO>> GetInterviewers()
        {
            var interviewers = await (from user in _con.Users
                                      join role in _con.Roles on user.RoleId equals role.RoleId
                                      where role.RoleName == "Interviewer" // Filter by RoleName
                                      select new UserRoleDTO
                                      {
                                          UserId = user.UserId,
                                          FirstName = user.FirstName,
                                          LastName = user.LastName,
                                          Email = user.Email,
                                          CreatedAt = user.CreatedAt,
                                          RoleId = role.RoleId,
                                          RoleName = role.RoleName,
                                          IsVerified = user.IsVerified
                                      }).ToListAsync();
            return interviewers;
        }
    }
}