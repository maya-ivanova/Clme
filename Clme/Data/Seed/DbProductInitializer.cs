using Clme.Data.Entities;
using Clme.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace Clme.Data.Seed
    {
    public static class DbProductInitializer
        {
        public static async Task SeedAsync(ApplicationDbContext context)
            {
            // Apply migrations automatically (DevOps)
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
                {
                await context.Database.MigrateAsync();
                }

            // 1. Seed Brands
            if (!await context.Brands.AnyAsync())
                {
                var brands = new List<Brand>
                {
                    new Brand { Name = "Daikin", Country = "Japan", LogoUrl = "daikin-logo.png" },
                    new Brand { Name = "Mitsubishi Electric", Country = "Japan", LogoUrl = "mitsubishi-logo.png" },
                    new Brand { Name = "Gree", Country = "China", LogoUrl = "gree-logo.png" }
                };
                await context.Brands.AddRangeAsync(brands);
                await context.SaveChangesAsync(); // Save to get Brand IDs
                }
           
            // 2. Seed Categories
            if (!await context.Categories.AnyAsync())
                {
                
                var categories = new List<Category>
                {
                    new Category { Name = "Wall Mounted" },
                    new Category { Name = "Floor Standing" },
                    new Category { Name = "Multi-Split" }
                };
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync(); // Save to get Category IDs
                }

            // 3. Seed Products
            if (!await context.Products.AnyAsync())
                {
                var daikin = await context.Brands.FirstAsync(b => b.Name == "Daikin");
                var wallMounted = await context.Categories.FirstAsync(c => c.Name == "Wall Mounted");

                await context.Products.AddRangeAsync(
                    new Product
                        {
                        BrandId = daikin.Id,
                        CategoryId = wallMounted.Id,
                        Model = "Ururu Sarara 9000",
                        Btu = 9000,
                        EnergyClass = EnergyClass.A___,
                        Price = 950.00m,
                        DiscountPercent = DiscountPercent.TwentyFive,
                        Description = "The ultimate air purifier and conditioner.",
                        IsAvailableAtStore = true
                        },
                    new Product
                        {
                        BrandId = daikin.Id,
                        CategoryId = wallMounted.Id,
                        Model = "Sensira 12000",
                        Btu = 12000,
                        EnergyClass = EnergyClass.A__,
                        Price = 1260.00m,
                        DiscountPercent = DiscountPercent.None,
                        Description = "Reliable and efficient climate control.",
                        IsAvailableAtStore = true
                        }
                );
                await context.SaveChangesAsync();
                }
            }
        }
    }
