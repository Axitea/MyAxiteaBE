using Microsoft.Extensions.DependencyInjection;
using MYA.Business.Auth;
using MYA.Business.Security;
using MYA.Business.MyCliente;

namespace MYA.Business;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<ILegacyPasswordCipher, LegacyPasswordCipher>();
        services.AddSingleton<IMfaCodeGenerator, MfaCodeGenerator>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMyCliente, MyInfoClienti>();
        return services;
    }
}
