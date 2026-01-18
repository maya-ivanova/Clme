using Clme.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Clme.Data.Entities
{
    public class Product
    {
        public int Id { get; set; }

        // Foreign Keys
        public int BrandId { get; set; }
        public int CategoryId { get; set; }

        // Navigation properties
        public Brand Brand { get; set; } = null!;
        public Category Category { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Model { get; set; } = null!;

        [Range(5000, 60000)]
        public int Btu { get; set; }

        public EnergyClass EnergyClass { get; set; }

        [Range(500, 10000)]
        [Precision(18, 2)]
        public decimal Price { get; set; }

        public DiscountPercent DiscountPercent { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public bool IsAvailableAtStore { get; set; } = true;
    }

}





