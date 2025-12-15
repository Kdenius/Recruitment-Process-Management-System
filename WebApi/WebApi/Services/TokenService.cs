using System;
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

                // Convert to Base64 URL-safe string
                var base64Token = Convert.ToBase64String(tokenBytes);

                // Replace characters to make it URL-safe
                var urlSafeToken = base64Token
                    .Replace('+', '-')   // Replace '+' with '-'
                    .Replace('/', '_')   // Replace '/' with '_'
                    .Replace("=", "");   // Remove the '=' padding character

                return urlSafeToken;
            }
        }
    }
}
