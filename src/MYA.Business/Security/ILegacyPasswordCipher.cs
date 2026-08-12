namespace MYA.Business.Security;

public interface ILegacyPasswordCipher
{
    string Encrypt(string plainText, string keyPhrase);

    string Decrypt(string cipherText, string keyPhrase);
}
