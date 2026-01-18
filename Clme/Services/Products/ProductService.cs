 using Clme.Data;
using Clme.Data.Entities;
using Clme.Models.DTOs.Product;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Clme.Services.Products
    {
    public class ProductService : IProductService
        {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
            {
            _context = context;
            }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
            {
            return await _context.Products
        .Where(p => p.IsAvailableAtStore)
        .Select(ToDto) // Clean!
        .ToListAsync();
            }

        public async Task<ProductDto?> GetByIdAsync(int id)
            {
            return await _context.Products
                .Where(p => p.Id == id && p.IsAvailableAtStore) // 1. Filter
                .Select(ToDto)
                .FirstOrDefaultAsync(); // 3. Execute (Get the one or null)
            }

        // reusable conversion expression
        private static readonly Expression<Func<Product, ProductDto>> ToDto = p => new ProductDto
            {
            Id = p.Id,
            Brand = p.Brand.Name,
            Category = p.Category.Name,
            Model = p.Model,
            Btu = p.Btu,
            EnergyClass = p.EnergyClass,
            Price = p.Price,
            DiscountPercent = p.DiscountPercent,
            IsAvailableAtStore = p.IsAvailableAtStore
            };
        }
    }

