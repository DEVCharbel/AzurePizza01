using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AzurePizza01.Data;
using AzurePizza01.DTOs;
using AzurePizza01.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzurePizza01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public OrdersController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // POST: api/orders
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto model)
        {
            if (model == null || model.Items == null || !model.Items.Any())
                return BadRequest(new { message = "Du måste skicka med minst en maträtt." });

            // Hämta inloggad användares Id
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            decimal totalPrice = 0m;
            var orderItems = new System.Collections.Generic.List<OrderItem>();

            foreach (var item in model.Items)
            {
                var matratt = await _dbContext.Matratter.FindAsync(item.MatrattId);
                if (matratt == null)
                    return BadRequest(new { message = $"Maträtt med id {item.MatrattId} finns ej." });

                totalPrice += matratt.Price * item.Quantity;

                orderItems.Add(new OrderItem
                {
                    MatrattId = item.MatrattId,
                    Quantity = item.Quantity
                });
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalPrice = totalPrice,
                OrderItems = orderItems
            };

            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            return StatusCode(201, new { order.Id, order.TotalPrice, order.OrderDate });
        }

        // GET: api/orders/mine
        [HttpGet("mine")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var orders = await _dbContext.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Matratt)
                .Select(o => new
                {
                    o.Id,
                    o.OrderDate,
                    o.TotalPrice,
                    Items = o.OrderItems.Select(oi => new
                    {
                        oi.MatrattId,
                        oi.Matratt.Name,
                        oi.Quantity,
                        Subtotal = oi.Matratt.Price * oi.Quantity
                    })
                })
                .ToListAsync();

            return Ok(orders);
        }
    }
}
