using System.Security.Cryptography;

namespace MYA.Business.Auth;

public sealed class MfaCodeGenerator
{
    public string Generate(int length)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "MFA code length must be positive.");
        }

        Span<char> buffer = stackalloc char[length];
        for (var i = 0; i < buffer.Length; i++)
        {
            buffer[i] = (char)('0' + RandomNumberGenerator.GetInt32(0, 10));
        }

        return new string(buffer);
    }
}
