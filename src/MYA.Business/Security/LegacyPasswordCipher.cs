using System.Security.Cryptography;
using System.Text;

namespace MYA.Business.Security;

public sealed class LegacyPasswordCipher : ILegacyPasswordCipher
{
    public string Encrypt(string plainText, string keyPhrase)
    {
        using var aes = CreateAes(keyPhrase);
        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        return Convert.ToBase64String(cipherBytes);
    }

    public string Decrypt(string cipherText, string keyPhrase)
    {
        using var aes = CreateAes(keyPhrase);
        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        var cipherBytes = Convert.FromBase64String(cipherText);
        var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }

    private static Aes CreateAes(string keyPhrase)
    {
        var key = Encoding.UTF8.GetBytes(keyPhrase);
        if (key.Length is not (16 or 24 or 32))
        {
            throw new InvalidOperationException("Legacy password key phrase must be 16, 24, or 32 UTF-8 bytes.");
        }

        var aes = Aes.Create();
        aes.Key = key;
        aes.IV = new byte[16];
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        return aes;
    }
}
