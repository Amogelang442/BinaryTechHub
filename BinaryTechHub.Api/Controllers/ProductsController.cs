using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BinaryTechHub.Api.Data;
using BinaryTechHub.Api.Models;

namespace BinaryTechHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    // Single constructor injecting your EF Core DbContext
    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        var products = await _context.Products.ToListAsync();
        return Ok(products);
    }

    // GET: api/products/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(string id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    // PATCH: api/products/{id}/stock
    [HttpPatch("{id}/stock")]
    public async Task<IActionResult> UpdateProductStock(string id, [FromBody] UpdateStockDto dto)
    {
        // 1. Security header validation
        if (!Request.Headers.TryGetValue("X-Admin-Key", out var adminKey) || adminKey != "BTH-Secret-Admin-2026")
        {
            return Unauthorized(new { message = "Unauthorized: Invalid or missing administrative key." });
        }

        // 2. Business validation
        if (dto == null || dto.NewStock < 0)
        {
            return BadRequest(new { message = "Stock quantity must be a non-negative number." });
        }

        try
        {
            // 3. Find the product via Entity Framework
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound(new { message = $"Product '{id}' not found." });
            }

            // 4. Update and persist change
            product.Stock = dto.NewStock;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Stock updated successfully.",
                productId = id,
                updatedStock = dto.NewStock
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Database error.", error = ex.Message });
        }
    }
}

public class UpdateStockDto
{
    public int NewStock { get; set; }
}