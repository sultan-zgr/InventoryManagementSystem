using System;
using System.Security.Cryptography;

namespace InventoryManagementSystem.Infrastructure.Services
{
    public interface ITokenGenerator
    {
        string GenerateToken();
    }

    public class TokenGenerator : ITokenGenerator
    {
        public string GenerateToken()
        {
            using var rng = new RNGCryptoServiceProvider();
            var randomBytes = new byte[32];
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
