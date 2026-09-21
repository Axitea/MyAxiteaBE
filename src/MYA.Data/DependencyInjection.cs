using Microsoft.Extensions.DependencyInjection;
using MYA.Data.Common;
using MYA.Data.DbUnico;
using MYA.Data.Puzzle;
using MYA.Data.Sat;

namespace MYA.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services)
    {
        services.AddSingleton<SqlConnectionFactory>();
        services.AddScoped<SqlExecutor>();
        services.AddScoped<PuzzleDataAccess>();
        services.AddScoped<SatDataAccess>();
        services.AddScoped<DbUnicoDataAccess>();

        return services;
    }
}
