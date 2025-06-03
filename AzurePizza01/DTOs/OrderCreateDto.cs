using System.Collections.Generic;

namespace AzurePizza01.DTOs
{
    public class OrderCreateDto
    {
        public List<OrderItemDto> Items { get; set; }
    }
}