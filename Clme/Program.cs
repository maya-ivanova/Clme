using Clme.Data;
using Clme.Data.Entities;
using Clme.Data.Seed;
using Clme.Middleware;
using Clme.ProjectSystems.HealthChecks;
using Clme.Services.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// 1. CONFIGURATION
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
    {
    throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
    }

// 2. SERVICE REGISTRATION
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IProductService, ProductService>();

// 3. HEALTH CHECKS
var healthBuilder = builder.Services.AddHealthChecks()
    .AddCheck("uptime", () =>
    {
        var uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
        return HealthCheckResult.Healthy($"Uptime: {uptime}");
    })
    .AddSqlServer(connectionString, name: "sql", failureStatus: HealthStatus.Unhealthy)
    .AddCheck<EfMigrationHealthCheck>("ef_migrations");

// 4. BUILD APP
var app = builder.Build();

// 5. MIDDLEWARE PIPELINE
app.UseMiddleware<GlobalExceptionMiddleware>();

if (!app.Environment.IsDevelopment())
    {
    app.UseHsts();
    }

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 6. SEEDING
using (var scope = app.Services.CreateScope())
    {
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

    var pending = await context.Database.GetPendingMigrationsAsync();
    if (pending.Any())
        {
        await context.Database.MigrateAsync();
        }

    await DbProductInitializer.SeedAsync(context);
    }

// 7. HEALTH ENDPOINT
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
