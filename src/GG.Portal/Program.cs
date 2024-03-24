using Destructurama;
using GG.Portal.Components;
using GG.Portal.Components.Account;
using GG.Portal.Data;
using GG.Portal.Enums;
using GG.Portal.Filters;
using GG.Portal.Logging;
using GG.Portal.Services.Account;
using GG.Portal.Services.ApiKey;
using GG.Portal.Services.AppConfig;
using GG.Portal.Services.AppEmail;
using GG.Portal.Services.Email;
using GG.Portal.Services.EmailTemplate;
using GG.Portal.Services.SecretKeyEncryption;
using GG.Portal.Services.Setup;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args);

var configurationManager = builder.Configuration;

var msSqlConnection = configurationManager.GetConnectionString("MsSqlConnection");
var mySqlConnection = configurationManager.GetConnectionString("MySqlConnection") ?? string.Empty;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var configurationService = new AppConfigService(configuration);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var services = builder.Services;

services.AddCascadingAuthenticationState();
services.AddScoped<IdentityUserAccessor>();
services.AddScoped<IdentityRedirectManager>();
services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

services.AddScoped<AccountService>();
services.AddScoped<AppEmailService>();
services.AddScoped<AppEmailTemplateService>();
services.AddScoped<EmailService>();
services.AddScoped<EmailTemplateService>();
services.AddScoped<SecretKeyEncryptionService>();
services.AddScoped<AppConfigService>();
services.AddScoped<ApiKeyService>();
services.AddScoped<SetupService>();
services.AddScoped<ApiKeyAuthFilter>();
services.AddTransient<UserNameEnricher>();
services.AddTransient<JsInterop>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

services.AddDbContext<ApplicationDbContext>(
    options =>
    {
        _ = configurationService.AppConfig.DatabaseType switch
        {
            AppConfigService.MsSqlDatabaseType => options.UseSqlServer(msSqlConnection),

            AppConfigService.MySqlDatabaseType => options.UseMySQL(mySqlConnection),

            _ => throw new Exception($"Unsupported database provider: {configurationService.AppConfig.DatabaseType}")
        };
    });


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = configurationService.UserPasswordConfig.RequireDigit;
    options.Password.RequireLowercase = configurationService.UserPasswordConfig.RequireLowercase;
    options.Password.RequireUppercase = configurationService.UserPasswordConfig.RequireUppercase;
    options.Password.RequireNonAlphanumeric = configurationService.UserPasswordConfig.RequireNonAlphanumeric;
    options.Password.RequiredLength = configurationService.UserPasswordConfig.RequiredLength;
})
.AddSignInManager()
.AddRoles<UserRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAntDesign();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

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
    //.WriteTo.Conditional(evt => configurationService.IsDatabaseTypeMySql(),
    //    wt => wt.MariaDB(
    //        connectionString: configurationManager.GetConnectionString("MySqlConnection"),
    //        tableName: "Logs"))
);

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DisplayOperationId();
    });

    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.MapControllers();

app.Run();
