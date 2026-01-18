using System;

namespace Clme.Data.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int? ParentCategoryId { get; set; } // future: Solar → Panels

        //public ParentCategory ParentCategory { get; set; } = null!;
    }
}
