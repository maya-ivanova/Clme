using Clme.Models.DTOs.Product;

namespace Clme.Models.ViewModels.Products
{
    public class ProductDetailsViewModel
    {
        // We wrap the DTO here
        public ProductDto Product { get; set; } = null!;

        // Add UI-specific properties that aren't in the database
        public string PageTitle => $"{Product.Brand} {Product.Model} - Details";

        public decimal FinalPrice => Product.Price * (1 - (decimal)Product.DiscountPercent / 100);

        // Chic extra: A flag to show a "Hot Deal" badge if discount > 20%
        public bool IsHotDeal => (int)Product.DiscountPercent >= 20;
    }
}