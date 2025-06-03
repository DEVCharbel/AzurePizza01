namespace AzurePizza01.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int MatrattId { get; set; }
        public Matratt Matratt { get; set; }

        public int Quantity { get; set; }
    }
}