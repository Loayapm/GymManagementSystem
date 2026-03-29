using System;
using System.Security.Cryptography;
using System.Text;

public static class PasswordHelper
{
    private const int SaltSize = 16;          // 128 bits
    private const int HashSize = 32;          // 256 bits output
    private const int Iterations = 600_000;     // strong for local desktop app (~0.2–0.8 s on modern PC)

    public static string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be empty.");

        byte[] salt = new byte[SaltSize]; //reserve space for salt
        using (var rng = new RNGCryptoServiceProvider())   // older name, still works everywhere
        {
            rng.GetBytes(salt); // fill salt with cryptographically strong random bytes
        }

        byte[] hash; // reserve space for hash
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
        {
            hash = pbkdf2.GetBytes(HashSize); // compute the hash of the password using PBKDF2 with the specified parameters
        }

        // Format: iterations:saltBase64:hashBase64
        return $"{Iterations}:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}"; // store iterations, salt and hash together in a single string for later verification
    }

    public static bool VerifyPassword(string password, string storedHash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
            return false; // empty password

        string[] parts = storedHash.Split(':');
        if (parts.Length != 3)
            return false; // the stored info are not complete (missing iterations, salt or hash) 

        if (!int.TryParse(parts[0], out int iterations) || iterations < 100_000)
            return false; // invalid or too low iteration count (should be at least 100k for security, but we use 600k by default)

        byte[] salt;
        byte[] expectedHash;
        try
        {
            salt = Convert.FromBase64String(parts[1]); // decode the salt from Base64 string to byte array
            expectedHash = Convert.FromBase64String(parts[2]); // decode the expected hash from Base64 string to byte array
        }
        catch
        {
            return false; // invalid Base64 format for salt or hash
        }

        if (salt.Length != SaltSize || expectedHash.Length != HashSize)
            return false; // invalid salt or hash size (should match the sizes defined in the class)

        byte[] actualHash;
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
        {
            actualHash = pbkdf2.GetBytes(HashSize); // compute the hash of the provided password using the same parameters (salt and iterations) as when it was stored
        }

        // Constant-time comparison (manual version that works everywhere)
        if (actualHash.Length != expectedHash.Length)
            return false;

        int diff = 0;
        for (int i = 0; i < actualHash.Length; i++)
        {
            diff |= actualHash[i] ^ expectedHash[i]; // XOR each byte of the actual hash with the expected hash and accumulate the differences in a single variable (diff will be 0 if all bytes match, otherwise it will be non-zero)
        }
        return diff == 0;
    }
}