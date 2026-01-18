using Clme.Data;
using Clme.Data.Entities;
using Clme.Data.Seed;
using Clme.Middleware;
using Clme.ProjectSystems.HealthChecks;
using Clme.Services.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Runtime.InteropServices;

// 1. ENVIRONMENT DETECTION (The "Backender" Prep)
bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
bool isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
bool isGitHubActions = Environment.GetEnvironmentVariable("CI") == "true";
bool isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. SERVICE REGISTRATION (The Blueprint)
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IProductService, ProductService>();

// 3. CONSOLIDATED HEALTH CHECKS (Registered BEFORE .Build())
var healthBuilder = builder.Services.AddHealthChecks()
    .AddSqlServer(connectionString!, name: "sql", failureStatus: HealthStatus.Unhealthy)
    .AddCheck("uptime", () =>
    {
        var uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
        return HealthCheckResult.Healthy($"Uptime: {uptime}");
    })
    .AddCheck<EfMigrationHealthCheck>("ef_migrations");

// Environment-specific Disk Checks
if (isWindows && builder.Environment.IsDevelopment())
{
    healthBuilder.AddDiskStorageHealthCheck(opt => opt.AddDrive("C:\\", 500), name: "disk");
}
else if (isLinux && (isDocker || isGitHubActions))
{
    // Simplified: Both Docker and GH Actions use "/" on Linux
    int threshold = isGitHubActions ? 100 : 200;
    healthBuilder.AddDiskStorageHealthCheck(opt => opt.AddDrive("/", threshold), name: "disk");
}

// NOW we build
var app = builder.Build();

// 4. THE MIDDLEWARE PIPELINE (The Execution)

// The "Airbag"
app.UseMiddleware<GlobalExceptionMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// Configure the HTTP request pipeline
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// 5. SEEDING (Clean and Async)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

    // Check migrations before seeding
    var pending = await context.Database.GetPendingMigrationsAsync();
    if (pending.Any())
    {
        // For a graduation project, it's chic to auto-apply them
        await context.Database.MigrateAsync();
    }

    await DbProductInitializer.SeedAsync(context);
}

// 6. MAP HEALTH ENDPOINT (With your custom JSON writer)
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description
            }),
            server_uptime = TimeSpan.FromMilliseconds(Environment.TickCount64).ToString()
        });
        await context.Response.WriteAsync(result);
    }
});

app.Run();