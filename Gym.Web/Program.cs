using System.Reflection;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Gym.Infrastructure;
using Gym.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DetailedErrors = builder.Environment.IsDevelopment();
        options.DisconnectedCircuitMaxRetained = 100;
        options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
        options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(1);
    })
    .AddHubOptions(options =>
    {
        options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
        options.EnableDetailedErrors = builder.Environment.IsDevelopment();
        options.KeepAliveInterval = TimeSpan.FromSeconds(15);
        options.MaximumParallelInvocationsPerClient = 1;
        options.MaximumReceiveMessageSize = 32 * 1024;
    });

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 1000,
                QueueLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
});

// Register infrastructure services (DbContext, Repositories, etc.)
builder.Services.InfrastructureConfiguration(builder.Configuration);
builder.Services.AddScoped<Gym.Web.Services.ToastService>();
builder.Services.AddScoped<Gym.Web.Services.ConfirmService>();
builder.Services.AddScoped<Gym.Web.Services.SecurityService>();

// Add Custom Auth
builder.Services.AddAuthentication("Cookies").AddCookie("Cookies", options =>
{
    options.LoginPath = "/login";
});
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

// Register AutoMapper (AutoMapper 15.1.1)
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<Gym.Infrastructure.Mapping.MappingProfile>();
});

var app = builder.Build();

app.UseResponseCompression();
app.UseRateLimiter();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapPost("/api/auth/login", async (HttpContext context, [Microsoft.AspNetCore.Mvc.FromForm] string password, Gym.Web.Services.SecurityService securityService) =>
{
    if (await securityService.VerifyPasswordAsync(password))
    {
        var identity = new System.Security.Claims.ClaimsIdentity(new[] { new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "Admin") }, "Cookies");
        var user = new System.Security.Claims.ClaimsPrincipal(identity);
        await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignInAsync(context, "Cookies", user, new Microsoft.AspNetCore.Authentication.AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
        });
        return Microsoft.AspNetCore.Http.Results.Redirect("/");
    }
    return Microsoft.AspNetCore.Http.Results.Redirect("/login?error=true");
}).DisableAntiforgery();

app.MapPost("/api/auth/logout", async (HttpContext context) =>
{
    await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignOutAsync(context, "Cookies");
    return Microsoft.AspNetCore.Http.Results.Redirect("/login");
}).DisableAntiforgery();

app.Run();
