using GG.ComingSoon.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GG.ComingSoon.Core.Config;

public static class ServiceCollection
{
    private const string MsSqlConnection = "MsSqlConnection";
    private const string MySqlConnection = "MySqlConnection";

    public static void AddComingSoon(this IServiceCollection services, IHostApplicationBuilder builder, IConfiguration configuration)
    {
        var configurationManager = builder.Configuration;
        var configService = new ConfigurationService(configuration);

        services.AddDbContext<EmailSubscriptionDbContext>(
            options => _ = configService.DatabaseType switch
            {
                ConfigurationService.MsSqlDatabaseType => options.UseSqlServer(
                    configurationManager.GetConnectionString(MsSqlConnection)),

                ConfigurationService.MySqlDatabaseType => options.UseMySQL(
                configurationManager.GetConnectionString(MySqlConnection)),

                _ => throw new Exception($"Unsupported database provider: {configService.DatabaseType}")
            });

        services.AddScoped<EmailSubscriptionService>();
    }
}
