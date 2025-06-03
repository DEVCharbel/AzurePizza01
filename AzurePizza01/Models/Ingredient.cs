namespace AzurePizza01.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int MatrattId { get; set; }
        public Matratt Matratt { get; set; }
    }
}