using GG.ComingSoon.Core.Config;
using Microsoft.Extensions.Configuration;

namespace GG.ComingSoon.Core;

public class ConfigurationService
{
    private readonly IConfiguration configuration;
    private readonly AppConfig appConfig = new();

    public const string MsSqlDatabaseType = "MsSql";
    public const string MySqlDatabaseType = "MySql";

    public string DatabaseType => appConfig.DatabaseType;

    public ConfigurationService(IConfiguration configuration)
    {
        this.configuration = configuration;
        this.configuration.GetSection(nameof(AppConfig)).Bind(appConfig);
    }

    public bool IsDatabaseTypeMsSql()
    {
        return DatabaseType == MsSqlDatabaseType;
    }

    public bool IsDatabaseTypeMySql()
    {
        return DatabaseType == MySqlDatabaseType;
    }
}
