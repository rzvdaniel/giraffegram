using GG.Auth;
using GG.Auth.Config;
using GG.Auth.Entities;
using GG.Auth.Services;
using GG.Core;
using GG.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Validation.AspNetCore;
using Quartz;

var builder = WebApplication.CreateBuilder(args);
var configurationManager = builder.Configuration;

var msSqlConnection = configurationManager.GetConnectionString("MsSqlConnection");
var mySqlConnection = configurationManager.GetConnectionString("MySqlConnection")??string.Empty;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
.Build();

var configurationService = new AppConfigService(configuration);

var services = builder.Services;

services.AddTransient<AccountService>();
services.AddTransient<AuthorizationService>();
services.AddTransient<EmailService>();
services.AddTransient<EmailAccountService>();
services.AddTransient<EmailTemplateService>();
services.AddTransient<SecretKeyEncryptionService>();
services.AddTransient<AppConfigService>();

services.AddControllers();
services.AddHttpContextAccessor();

services.AddAuth(builder, configuration);

services.AddDbContext<ApplicationDbContext>(
    options =>
    {
        _ = configurationService.DatabaseType switch
        {
            AppConfigService.MsSqlDatabaseType => options.UseSqlServer(msSqlConnection, x => x.MigrationsAssembly("GG.Migrations.MsSql")),

            AppConfigService.MySqlDatabaseType => options.UseMySQL(mySqlConnection, x => x.MigrationsAssembly("GG.Migrations.MySql")),

            _ => throw new Exception($"Unsupported database provider: {configurationService.DatabaseType}")
        };
    });

services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DisplayOperationId();
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
