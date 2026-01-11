using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Clme.Data.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Brand { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Model { get; set; } = null!;

        [Range(500, 10000)]
        [Precision(18, 2)]
        public decimal Price { get; set; }

        [Range(5000, 60000)]
        public int Btu { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }
    }
}