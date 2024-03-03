using Destructurama;
using GG.Api;
using GG.Auth.Config;
using GG.Auth.Services;
using GG.Core;
using GG.Core.Authentication;
using GG.Core.Services;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.MSSqlServer;

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
services.AddTransient<EmailTemplateService>();
services.AddTransient<SecretKeyEncryptionService>();
services.AddTransient<AppConfigService>();
services.AddTransient<ApiKeyService>();
services.AddTransient<SetupService>();
services.AddTransient<ApiKeyAuthFilter>();
services.AddTransient<AppEmailService>();
services.AddTransient<UserNameEnricher>();

services.AddControllers();
services.AddHttpContextAccessor();

services.AddExceptionHandler<GlobalExceptionHandler>();
services.AddProblemDetails();

services.AddAuth(builder, configuration);

services.AddDbContext<ApplicationDbContext>(
    options =>
    {
        _ = configurationService.AppConfig.DatabaseType switch
        {
            AppConfigService.MsSqlDatabaseType => options.UseSqlServer(msSqlConnection, x => x.MigrationsAssembly("GG.Migrations.MsSql")),

            AppConfigService.MySqlDatabaseType => options.UseMySQL(mySqlConnection, x => x.MigrationsAssembly("GG.Migrations.MySql")),

            _ => throw new Exception($"Unsupported database provider: {configurationService.AppConfig.DatabaseType}")
        };
    });

Serilog.Debugging.SelfLog.Enable(Console.Out);

builder.Host.UseSerilog((context, services, config) =>
    config.Destructure.UsingAttributes()
    .ReadFrom.Configuration(configuration)
    .Destructure.JsonNetTypes()
    .Enrich.With<EventTypeEnricher>()
    .Enrich.With(services.GetService<UserNameEnricher>()!)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .WriteTo.Conditional(evt => configurationService.IsDatabaseTypeMsSql(),
        wt => wt.MSSqlServer(
            //If provided, the settings of MSSqlServerSinkOptions and ColumnOptions
            //objects created in code are treated as a baseline
            //which is then updated from the external configuration data
            //https://github.com/serilog-mssql/serilog-sinks-mssqlserver
            connectionString: configurationManager.GetConnectionString("MsSqlConnection"),
            appConfiguration: configuration,
            // Below configuration is overritten by configuration from appsettings.json
            logEventFormatter: new CompactJsonFormatter(),
            sinkOptions: new MSSqlServerSinkOptions { TableName = "Logs" }))
);

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

app.UseSerilogRequestLogging(
    options =>
    {
        options.MessageTemplate = "{ClientIP} {RequestScheme} {RequestHost} {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress);
            // TODO! Need to send UserAgent from Blazor application with each HttpClient request
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].FirstOrDefault());
        };
    });

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();
