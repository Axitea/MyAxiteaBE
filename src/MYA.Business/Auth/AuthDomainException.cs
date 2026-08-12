namespace MYA.Business.Auth;

public sealed class AuthDomainException : Exception
{
    public AuthDomainException(string message)
        : base(message)
    {
    }
}
