using Clme.Data.Entities;
using Clme.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Clme.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
            base.OnModelCreating(modelBuilder);

            // Convert Enums to Strings in the Database to make them readable from any OS
            modelBuilder.Entity<Product>()
                .Property(p => p.EnergyClass)
                .HasConversion<string>();

            modelBuilder.Entity<Product>()
                .Property(p => p.DiscountPercent)
                .HasConversion<string>();

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);
            }
        }
    }