using Clme.Data;
using Clme.Data.Entities;
using Clme.Data.Seed;
using Clme.Middleware;
using Clme.Services.Products;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. CONFIGURATION: choose DB provider by environment
if (builder.Environment.IsDevelopment() || builder.Environment.IsStaging())
    {
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite("Data Source=AirCondsOnlineShop.db"));
    }
else if (builder.Environment.IsProduction())
    {
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Production connection string not configured.");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
    }

// 2. SERVICE REGISTRATION
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IProductService, ProductService>();

// 3. BUILD APP
var app = builder.Build();

// 4. MIDDLEWARE PIPELINE
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

// 5. SEEDING
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

app.Run();
