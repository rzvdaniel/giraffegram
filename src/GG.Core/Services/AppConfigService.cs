using GG.Core.Config;
using Microsoft.Extensions.Configuration;

namespace GG.Core.Services;

public class AppConfigService
{
    private readonly IConfiguration configuration;
    private readonly AppConfig appConfig = new();

    public const string MsSqlDatabaseType = "MsSql";
    public const string MySqlDatabaseType = "MySql";

    public string DatabaseType => appConfig.DatabaseType??
        throw new InvalidOperationException("DatabaseType configuration not found");

    public string UserEncryptionKey => appConfig.UserEncryptionKey ?? 
        throw new InvalidOperationException("UserEncryptionKey configuration not found");

    public AppConfigService(IConfiguration configuration)
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
