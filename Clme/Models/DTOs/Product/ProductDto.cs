using Clme.Data.Enums;

namespace Clme.Models.DTOs.Product
    {
    public class ProductDto
        {
        public int Id { get; set; }

        public string Brand { get; set; } = null!;
        public string Category { get; set; } = null!;

        public string Model { get; set; } = null!;
        public int Btu { get; set; }

        public EnergyClass EnergyClass { get; set; }

        public decimal Price { get; set; }
        public DiscountPercent DiscountPercent { get; set; }

        public bool IsAvailableAtStore { get; set; }
        }

    }
