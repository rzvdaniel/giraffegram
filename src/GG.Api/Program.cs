using GG.Auth;
using GG.Auth.Entities;
using GG.Auth.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
services.AddTransient<UserManagerService>();

services.AddDbContext<ApplicationDbContext>(
    options => _ = configurationService.DatabaseType switch
    {
        ConfigurationService.MsSqlDatabaseType => options.UseSqlServer(msSqlConnection),

        ConfigurationService.MySqlDatabaseType => options.UseMySQL(mySqlConnection),

        _ => throw new Exception($"Unsupported database provider: {configurationService.DatabaseType}")
    });

services.AddDatabaseDeveloperPageExceptionFilter();

services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireDigit = configurationService.RequireDigit;
    options.Password.RequireLowercase = configurationService.RequireLowercase;
    options.Password.RequireUppercase = configurationService.RequireUppercase;
    options.Password.RequireNonAlphanumeric = configurationService.RequireNonAlphanumeric;
    options.Password.RequiredLength = configurationService.RequiredLength;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
