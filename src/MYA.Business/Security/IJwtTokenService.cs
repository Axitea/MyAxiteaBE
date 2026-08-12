using MYA.Models.Auth;

namespace MYA.Business.Security;

public interface IJwtTokenService
{
    IssuedToken Issue(LoginUser user, int appId, DateTimeOffset now);
}
