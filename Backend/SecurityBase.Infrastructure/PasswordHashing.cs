using System.Security.Cryptography;

namespace SecurityBase.Infrastructure;

public static class PasswordHashing
{
    private const int SaltSize = 16; // 128-bit
    private const int KeySize = 32;  // 256-bit
    private const int Iterations = 100_000;

    // Format: PBKDF2$SHA256$<iterations>$<saltB64>$<keyB64>
    private const string Prefix = "PBKDF2$SHA256$";

    public static string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required.", nameof(password));

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize
        );

        return $"{Prefix}{Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(key)}";
    }

    public static bool VerifyPassword(string storedHash, string password)
    {
        if (string.IsNullOrEmpty(storedHash) || string.IsNullOrEmpty(password))
            return false;

        // Back-compat: previously stored plain-text (NOT recommended)
        if (!storedHash.StartsWith(Prefix, StringComparison.Ordinal))
            return string.Equals(storedHash, password, StringComparison.Ordinal);

        var parts = storedHash.Split('$', StringSplitOptions.RemoveEmptyEntries);
        // Expected: PBKDF2 | SHA256 | iterations | saltB64 | keyB64
        if (parts.Length != 5)
            return false;

        if (!int.TryParse(parts[2], out var iterations) || iterations <= 0)
            return false;

        byte[] salt;
        byte[] expectedKey;
        try
        {
            salt = Convert.FromBase64String(parts[3]);
            expectedKey = Convert.FromBase64String(parts[4]);
        }
        catch
        {
            return false;
        }

        var actualKey = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            expectedKey.Length
        );

        return CryptographicOperations.FixedTimeEquals(expectedKey, actualKey);
    }

    public static bool IsHashed(string storedHash) =>
        !string.IsNullOrEmpty(storedHash) && storedHash.StartsWith(Prefix, StringComparison.Ordinal);
}

