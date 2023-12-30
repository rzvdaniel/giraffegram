using GG.Auth.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GG.Auth.Config;

public static class ServiceCollection
{
    public static void AddAuth(this IServiceCollection services, IHostApplicationBuilder builder, IConfiguration configuration)
    {
        var configurationManager = builder.Configuration;

        var msSqlConnection = configurationManager.GetConnectionString("MsSqlConnection");
        var mySqlConnection = configurationManager.GetConnectionString("MySqlConnection") ?? string.Empty;

        var configurationService = new ConfigurationService(configuration);

        services.AddDbContext<AuthDbContext>(
        options =>
        {
            _ = configurationService.DatabaseType switch
            {
                // TODO! Pass migration assemblies as parameters
                ConfigurationService.MsSqlDatabaseType => options.UseSqlServer(msSqlConnection, x => x.MigrationsAssembly("GG.Migrations.MsSql")),

                ConfigurationService.MySqlDatabaseType => options.UseMySQL(mySqlConnection, x => x.MigrationsAssembly("GG.Migrations.MySql")),

                _ => throw new Exception($"Unsupported database provider: {configurationService.DatabaseType}")
            };

            // Register the entity sets needed by OpenIddict.
            // Note: use the generic overload if you need
            // to replace the default OpenIddict entities.
            options.UseOpenIddict();
        });
    }
}
