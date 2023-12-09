using GG.ComingSoon.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GG.ComingSoon.Core.Config;

public static class SeviceCollection
{
    public static void AddComingSoon(this IServiceCollection services, IHostApplicationBuilder builder, IConfiguration configuration)
    {
        var configurationManager = builder.Configuration;
        var configService = new ConfigurationService(configuration);

        services.AddDbContext<EmailSubscriptionDbContext>(
            options => _ = configService.DatabaseType switch
            {
                ConfigurationService.MsSqlDatabaseType => options.UseSqlServer(
                    configurationManager.GetConnectionString("MsSqlConnection"),
                    x => x.MigrationsAssembly("Automaton.Studio.Server.MsSql.Migrations")),

                ConfigurationService.MySqlDatabaseType => options.UseMySQL(
                configurationManager.GetConnectionString("MySqlConnection"),
                    x => x.MigrationsAssembly("Automaton.Studio.Server.MySql.Migrations")),

                _ => throw new Exception($"Unsupported database provider: {configService.DatabaseType}")
            });

        services.AddScoped<EmailSubscriptionService>();
    }
}
