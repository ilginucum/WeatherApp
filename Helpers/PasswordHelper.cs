using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Weather_App.Helpers{ 
public static class PasswordHelper
{
    // Generate a new salt
    public static string GenerateSalt()
    {
        byte[] saltBytes = new byte[16];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
    }

    // Hash a password with a salt
    public static string HashPassword(string password, string salt)
    {
        byte[] saltBytes = Convert.FromBase64String(salt);
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: saltBytes,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 32));
    }

    // Verify a password
    public static bool VerifyPassword(string password, string salt, string hashedPassword)
    {
        string hashedInputPassword = HashPassword(password, salt);
        return hashedInputPassword == hashedPassword;
    }
}
}