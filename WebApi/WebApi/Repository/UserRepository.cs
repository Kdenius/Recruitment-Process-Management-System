using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
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
    }
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _con;
        private readonly IRoleRepository _roleRepository;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;

        public UserRepository(AppDbContext con, IRoleRepository roleRepository, IJwtTokenService jwtTokenService)
        {
            _con = con;
            _roleRepository = roleRepository;
            _passwordHasher = new PasswordHasher<User>();
            _jwtTokenService = jwtTokenService;

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

            var response = new LoginResponseDTO()
            {
                Userid = user.UserId,
                Email = loginRequest.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoleId = user.RoleId,
                RoleName = roleName,
                JwtToken = _jwtTokenService.GenerateJwtToken(user.UserId, loginRequest.Email, roleName),
                RefreshToken = _jwtTokenService.GenerateRefreshToken()
            };
            return new ApiResponse<LoginResponseDTO>(success: true, message: $"Login Successfully", data: response);
        }

    }
}
/*
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApi.DTO;
using WebApi.Models;
using WebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Repository
{
    public interface IUserRepository
    {
        User AddUser(UserDTO userDTO);
        User GetById(int id);
        Task<LoginResponseDto> LoginAsync(LoginRequest loginRequest);
        bool VerifyToken(string token, int userId);
    }

    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _con;
        private readonly IRoleRepository _roleRepository;
        private readonly IJwtService _jwtService; // Assuming you have a JWT service
        private readonly PasswordHasher<User> _passwordHasher;

        public UserRepository(AppDbContext con, IRoleRepository roleRepository, IJwtService jwtService)
        {
            _con = con;
            _roleRepository = roleRepository;
            _jwtService = jwtService;
            _passwordHasher = new PasswordHasher<User>();
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
                VerifyToken = TokenGenerator.GenerateRandomizeToken(),
            };
            usr.PasswordHash = _passwordHasher.HashPassword(usr, userDTO.Password);
            _con.Users.Add(usr);
            _con.SaveChanges();
            return usr;
        }

        public User GetById(int id)
        {
            return _con.Users.SingleOrDefault(user => user.UserId == id);
        }

        public bool VerifyToken(string token, int userId)
        {
            var user = _con.Users.FirstOrDefault(user => user.UserId == userId);
            if (user.VerifyToken == token)
            {
                user.IsVerified = true;
                _con.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequest loginRequest)
        {
            // 1. Find User by Email and Password Verification
            var user = await _con.Users
                .FirstOrDefaultAsync(u => u.Email == loginRequest.Email && !u.IsDeleted);
            if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginRequest.Password) == PasswordVerificationResult.Failed)
            {
                // If no user found or invalid password
                return null; // You can return a specific response or throw an exception depending on your error handling
            }

            // 2. Get User Role Information
            var userRoleInfo = await _con.UserRoles
                .Where(ur => ur.UserId == user.UserId)
                .Join(_con.Roles, ur => ur.RoleID, r => r.RoleID, (ur, r) => new
                {
                    RoleID = ur.RoleID,
                    RoleName = r.RoleName
                })
                .FirstOrDefaultAsync();

            if (userRoleInfo == null)
            {
                return null; // Return null or handle case where no role is found
            }

            // 3. Generate JWT and Refresh Token
            var token = _jwtService.GenerateToken(user.UserId, user.Email, userRoleInfo.RoleName);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // 4. Save Refresh Token and Expiry Time in Database
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            _con.Users.Update(user);
            await _con.SaveChangesAsync();

            // 5. Get Permissions for User Role
            var permissions = await _con.RolePermissions
                .Where(rp => rp.RoleID == userRoleInfo.RoleID)
                .Join(_con.Permissions, rp => rp.PermissionId, p => p.Id, (rp, p) => p.Name)
                .Distinct()
                .ToListAsync();

            // 6. Return User Data with Token, RefreshToken, and Permissions
            var loginResponseDto = new LoginResponseDto
            {
                Id = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Mobile = user.Mobile,
                RoleID = userRoleInfo.RoleID,
                RoleName = userRoleInfo.RoleName,
                Token = token,
                RefreshToken = refreshToken,
                Permissions = permissions
            };

            return loginResponseDto;
        }
    }
}

*/