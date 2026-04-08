using System.Security.Cryptography;
using ICTMasterSuite.Application.Abstractions.Security;

namespace ICTMasterSuite.Infrastructure.Security;

public sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;

    public string Hash(string plainTextPassword) => HashStatic(plainTextPassword);

    public bool Verify(string plainTextPassword, string hash)
    {
        var parts = hash.Split(':');
        if (parts.Length != 3)
        {
            return false;
        }

        var salt = Convert.FromBase64String(parts[1]);
        var expected = Convert.FromBase64String(parts[2]);
        var actual = Rfc2898DeriveBytes.Pbkdf2(plainTextPassword, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }

    public static string HashStatic(string plainTextPassword)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(plainTextPassword, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return $"PBKDF2:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }
}
