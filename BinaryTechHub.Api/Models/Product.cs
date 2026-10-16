using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BinaryTechHub.Api.Models;

[Table("products")]
public class Product
{
    [Key]
    [Column("product_id")]
    public string ProductId { get; set; } = string.Empty;

    [Column("category_id")]
    public int CategoryId { get; set; }

    [Column("name")]
    [Required]
    public string Name { get; set; } = string.Empty;

    [Column("specs")]
    public string? Specs { get; set; }

    [Column("price", TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    [Column("image_url")]
    public string? ImageUrl { get; set; }

    [Column("stock")]
    public int Stock { get; set; }

    // Navigation back to Category
    [ForeignKey("CategoryId")]
    public Category? Category { get; set; }
}