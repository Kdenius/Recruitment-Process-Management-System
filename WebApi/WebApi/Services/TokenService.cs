using System.Security.Cryptography;

namespace WebApi.Services
{
    public class TokenGenerator
    {
        public static string GenerateRandomizeToken()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenBytes = new byte[32]; // 256-bit token
                rng.GetBytes(tokenBytes);
                return Convert.ToBase64String(tokenBytes);
            }
        }
    }
}
