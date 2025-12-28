using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Bcpg;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApi.Models;

namespace WebApi.Services
{
    public interface IJwtTokenService
    {
        string GenerateJwtToken(int userId, string mail, string role);
        string GenerateRefreshToken();
    }
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _config;

        public JwtTokenService(IConfiguration config)
        {
            _config = config;
        }
        public string GenerateJwtToken(int userId, string mail, string role)
        {
            //int userId = user.UserId;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, mail),
                new Claim(JwtRegisteredClaimNames.Jti, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };
            var token = new JwtSecurityToken(issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt16(_config["Jwt:ExpiresInMinutes"])),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
