using GG.Core.Config;
using Microsoft.Extensions.Configuration;

namespace GG.Core.Services;

public class AppConfigService
{
    private readonly IConfiguration configuration;

    public AppConfig AppConfig { get; private set; } = new();
    public EmailConfig EmailConfig { get; private set; } = new();

    public const string MsSqlDatabaseType = "MsSql";
    public const string MySqlDatabaseType = "MySql";

    public AppConfigService(IConfiguration configuration)
    {
        this.configuration = configuration;
        this.configuration.GetSection(nameof(AppConfig)).Bind(AppConfig);
        this.configuration.GetSection(nameof(EmailConfig)).Bind(EmailConfig);
    }

    public bool IsDatabaseTypeMsSql()
    {
        return AppConfig.DatabaseType == MsSqlDatabaseType;
    }

    public bool IsDatabaseTypeMySql()
    {
        return AppConfig.DatabaseType == MySqlDatabaseType;
    }
}
