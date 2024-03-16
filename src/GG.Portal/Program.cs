using GG.Portal.Components;
using GG.Portal.Components.Account;
using GG.Portal.Data;
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

var builder = WebApplication.CreateBuilder(args);

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
services.AddScoped<UserNameEnricher>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddAntDesign();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddScoped<JsInterop>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
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

app.Run();
