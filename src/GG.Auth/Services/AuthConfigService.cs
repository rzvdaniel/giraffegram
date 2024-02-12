using GG.Auth.Config;
using Microsoft.Extensions.Configuration;

namespace GG.Auth.Services;

public class AuthConfigService
{
    private readonly IConfiguration configuration;
    public AuthConfig AuthConfig { get; private set; } = new();
    private readonly UserPasswordConfig userPasswordConfig = new();

    public const string MsSqlDatabaseType = "MsSql";
    public const string MySqlDatabaseType = "MySql";

    public bool RequireDigit => userPasswordConfig.RequireDigit;
    public bool RequireLowercase => userPasswordConfig.RequireLowercase;
    public bool RequireUppercase => userPasswordConfig.RequireUppercase;
    public bool RequireNonAlphanumeric => userPasswordConfig.RequireNonAlphanumeric;
    public int RequiredLength => userPasswordConfig.RequiredLength;

    public AuthConfigService(IConfiguration configuration)
    {
        this.configuration = configuration;
        this.configuration.GetSection(nameof(AuthConfig)).Bind(AuthConfig);
        this.configuration.GetSection(nameof(UserPasswordConfig)).Bind(userPasswordConfig);
    }

    public bool IsDatabaseTypeMsSql()
    {
        return AuthConfig.DatabaseType == MsSqlDatabaseType;
    }

    public bool IsDatabaseTypeMySql()
    {
        return AuthConfig.DatabaseType == MySqlDatabaseType;
    }
}
