namespace Clme.Data.Entities
    {
    public class Brand
        {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Country { get; set; } = null!;

        public string LogoUrl { get; set; } = null!;
        }
    }
