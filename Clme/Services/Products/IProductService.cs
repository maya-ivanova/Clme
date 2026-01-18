using Clme.Data.Entities;
using Clme.Models.DTOs.Product;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clme.Services.Products
    {
    public interface IProductService
        {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        }
    }

