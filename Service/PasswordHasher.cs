using System.Security.Cryptography;
using System.Text;

namespace NitroRivals.Services;

public static class PasswordHasher
{
    private const int SaltSize = 32;      // matches VARBINARY(32)
    private const int HashSize = 32;      // fits VARBINARY(64)
    private const int Iterations = 100_000;

    /// <summary>Hashes a password with a fresh random salt (PBKDF2-SHA256).</summary>
    public static byte[] HashPassword(string password, out byte[] salt)
    {
        salt = RandomNumberGenerator.GetBytes(SaltSize);
        return Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
    }

    public static bool Verify(string password, byte[] hash, byte[] salt)
    {
        byte[] computed = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return CryptographicOperations.FixedTimeEquals(computed, hash);
    }

    /// <summary>Cryptographically random 6-digit code.</summary>
    public static string GenerateOtp() =>
        RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

    /// <summary>Salted (by email) SHA-256 of the OTP. Only this is stored.</summary>
    public static byte[] HashOtp(string otp, string email) =>
        SHA256.HashData(Encoding.UTF8.GetBytes($"{otp}:{email.Trim().ToLowerInvariant()}"));
}
