using Microsoft.Extensions.DependencyInjection;
using MYA.Business.Auth;
using MYA.Business.Clienti;
using MYA.Business.Puzzle;

namespace MYA.Business;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        services.AddSingleton<TimeProvider>(TimeProvider.System);
        services.AddSingleton<LegacyPasswordCipher>();
        services.AddSingleton<MfaCodeGenerator>();
        services.AddSingleton<JwtTokenService>();
        services.AddScoped<AuthService>();
        services.AddScoped<ClientiService>();
        services.AddScoped<PuzzleService>();
        return services;
    }
}
