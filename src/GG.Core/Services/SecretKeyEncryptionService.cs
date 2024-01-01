using System.Security.Cryptography;
using System.Text;

namespace GG.Core.Services;

/// <summary>
/// Original code by Code Maze
/// <see cref="https://code-maze.com/csharp-string-encryption-decryption"/>
/// </summary>

public class SecretKeyEncryptionService
{
    private readonly byte[] IV =
    {
        0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
        0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
    };

    public async Task<byte[]> EncryptAsync(string clearText, string passphrase)
    {
        using Aes aes = Aes.Create();
        aes.Key = DeriveKeyFromPassword(passphrase);
        aes.IV = IV;

        using MemoryStream output = new();
        using CryptoStream cryptoStream = new(output, aes.CreateEncryptor(), CryptoStreamMode.Write);

        await cryptoStream.WriteAsync(Encoding.UTF8.GetBytes(clearText));
        await cryptoStream.FlushFinalBlockAsync();

        return output.ToArray();
    }

    public async Task<string> DecryptAsync(byte[] encrypted, string passphrase)
    {
        using Aes aes = Aes.Create();
        aes.Key = DeriveKeyFromPassword(passphrase);
        aes.IV = IV;

        using MemoryStream input = new(encrypted);
        using CryptoStream cryptoStream = new(input, aes.CreateDecryptor(), CryptoStreamMode.Read);

        // By default, the StreamReader uses UTF-8 encoding.
        // To change the text encoding, pass the desired encoding as the second parameter.
        // For example, new StreamReader(cryptoStream, Encoding.Unicode).
        using StreamReader decryptReader = new(cryptoStream);
        string decryptedMessage = await decryptReader.ReadToEndAsync();

        return decryptedMessage;
    }

    private static byte[] DeriveKeyFromPassword(string password)
    {
        var emptySalt = Array.Empty<byte>();
        var iterations = 1000;
        var desiredKeyLength = 16; // 16 bytes equal 128 bits.
        var hashMethod = HashAlgorithmName.SHA384;
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        return Rfc2898DeriveBytes.Pbkdf2(passwordBytes, emptySalt, iterations, hashMethod, desiredKeyLength);
    }
}