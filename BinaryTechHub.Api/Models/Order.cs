using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BinaryTechHub.Api.Models;

[Table("orders")]
public class Order
{
    [Key]
    [Column("order_ref")]
    public string OrderRef { get; set; } = string.Empty;

    [Column("customer_name")]
    public string CustomerName { get; set; } = string.Empty;

    [Column("customer_email")]
    public string CustomerEmail { get; set; } = string.Empty;

    [Column("delivery_address")]
    public string DeliveryAddress { get; set; } = string.Empty;

    [Column("payment_method")]
    public string PaymentMethod { get; set; } = string.Empty;

    [Column("total_amount", TypeName = "decimal(10,2)")]
    public decimal TotalAmount { get; set; }

    [Column("order_date")]
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public List<OrderItem> OrderItems { get; set; } = new();
}

[Table("order_items")]
public class OrderItem
{
    [Key]
    [Column("item_id")]
    public int ItemId { get; set; }

    [Column("order_ref")]
    public string OrderRef { get; set; } = string.Empty;

    [Column("product_id")]
    public string ProductId { get; set; } = string.Empty;

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("price_at_purchase", TypeName = "decimal(10,2)")]
    public decimal PriceAtPurchase { get; set; }
}