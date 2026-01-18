using Clme.Models.DTOs.Product;

namespace Clme.Models.ViewModels.Products
    {
    public class ProductListViewModel
        {
        public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();

        // Filters (future-ready)
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public int? MinBtu { get; set; }
        public int? MaxBtu { get; set; }
        }

    }
