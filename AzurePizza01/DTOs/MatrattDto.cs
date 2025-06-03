using System.Collections.Generic;

namespace AzurePizza01.DTOs
{
    public class MatrattDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public List<string> Ingredients { get; set; }
    }
}