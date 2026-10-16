using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BinaryTechHub.Api.Data;
using BinaryTechHub.Api.Models;

namespace BinaryTechHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

    // POST: api/orders
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
    {
        if (dto == null || dto.Items.Count == 0)
        {
            return BadRequest("Order must contain at least one item.");
        }

        // Database transaction to keep stock and order items atomic
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Verify stock and update inventory
            foreach (var item in dto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                {
                    return NotFound($"Product with ID '{item.ProductId}' not found.");
                }

                if (product.Stock < item.Quantity)
                {
                    return BadRequest($"Insufficient stock for product '{product.Name}'. Available: {product.Stock}");
                }

                // Decrement stock
                product.Stock -= item.Quantity;
            }

            // 2. Map and save Order
            var order = new Order
            {
                OrderRef = dto.OrderRef,
                CustomerName = dto.CustomerName,
                CustomerEmail = dto.CustomerEmail,
                DeliveryAddress = dto.DeliveryAddress,
                PaymentMethod = dto.PaymentMethod,
                TotalAmount = dto.TotalAmount,
                OrderDate = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // 3. Map and save Order Items
            foreach (var item in dto.Items)
            {
                var orderItem = new OrderItem
                {
                    OrderRef = order.OrderRef,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    PriceAtPurchase = item.UnitPrice
                };

                _context.OrderItems.Add(orderItem);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { message = "Order placed successfully!", orderRef = order.OrderRef });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}