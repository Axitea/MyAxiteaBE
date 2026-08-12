using Microsoft.Extensions.DependencyInjection;
using MYA.Data.Database;
using MYA.Data.Repositories;

namespace MYA.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services)
    {
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
        services.AddScoped<IDbExecutor, SqlDbExecutor>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<ITokenRepository, TokenRepository>();
        services.AddScoped<ISmsOutboxRepository, SmsOutboxRepository>();

        return services;
    }
}
