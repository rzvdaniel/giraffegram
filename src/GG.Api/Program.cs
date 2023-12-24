using GG.Auth;
using GG.Auth.Entities;
using GG.Auth.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quartz;

var builder = WebApplication.CreateBuilder(args);
var configurationManager = builder.Configuration;

var msSqlConnection = configurationManager.GetConnectionString("MsSqlConnection");
var mySqlConnection = configurationManager.GetConnectionString("MySqlConnection")??string.Empty;

var configurationBuilder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var configurationService = new ConfigurationService(configurationBuilder);

builder.Services.AddControllers();

// Add services to the container.
var services = builder.Services;
services.AddTransient<AccountService>();
services.AddTransient<AuthorizationService>();

services.AddDbContext<ApplicationDbContext>(
    options =>
    {
        _ = configurationService.DatabaseType switch
        {
            ConfigurationService.MsSqlDatabaseType => options.UseSqlServer(msSqlConnection, x => x.MigrationsAssembly("GG.Migrations.MsSql")),

            ConfigurationService.MySqlDatabaseType => options.UseMySQL(mySqlConnection, x => x.MigrationsAssembly("GG.Migrations.MySql")),

            _ => throw new Exception($"Unsupported database provider: {configurationService.DatabaseType}")
        };

        // Register the entity sets needed by OpenIddict.
        // Note: use the generic overload if you need
        // to replace the default OpenIddict entities.
        options.UseOpenIddict();
    });

services.AddDatabaseDeveloperPageExceptionFilter();

services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = configurationService.RequireDigit;
    options.Password.RequireLowercase = configurationService.RequireLowercase;
    options.Password.RequireUppercase = configurationService.RequireUppercase;
    options.Password.RequireNonAlphanumeric = configurationService.RequireNonAlphanumeric;
    options.Password.RequiredLength = configurationService.RequiredLength;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// OpenIddict offers native integration with Quartz.NET to perform scheduled tasks
// (like pruning orphaned authorizations/tokens from the database) at regular intervals.
services.AddQuartz(options =>
{
    options.UseSimpleTypeLoader();
    options.UseInMemoryStore();
});

// Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

services.AddOpenIddict()

// Register the OpenIddict core components.
.AddCore(options =>
{
    // Configure OpenIddict to use the Entity Framework Core stores and models.
    // Note: call ReplaceDefaultEntities() to replace the default OpenIddict entities.
    options.UseEntityFrameworkCore()
            .UseDbContext<ApplicationDbContext>();

    // Enable Quartz.NET integration.
    options.UseQuartz();
})

// Register the OpenIddict server components.
.AddServer(options =>
{
    options.SetTokenEndpointUris("api/authorization/token")
        .SetUserinfoEndpointUris("api/account/userinfo");

    options.AllowPasswordFlow()
        .AllowRefreshTokenFlow();

    // Accept anonymous clients (i.e clients that don't send a client_id).
    options.AcceptAnonymousClients();

    // Register the signing and encryption credentials.
    options.AddDevelopmentEncryptionCertificate()
        .AddDevelopmentSigningCertificate();

    // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
    options.UseAspNetCore()
        .EnableTokenEndpointPassthrough()
        .EnableUserinfoEndpointPassthrough();
})

// Register the OpenIddict validation components.
.AddValidation(options =>
{
    // Import the configuration from the local OpenIddict server instance.
    options.UseLocalServer();

    // Register the ASP.NET Core host.
    options.UseAspNetCore();
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
