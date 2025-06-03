using System.Collections.Generic;

namespace AzurePizza01.Models
{
    public class Matratt
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public ICollection<Ingredient> Ingredients { get; set; }
    }
}