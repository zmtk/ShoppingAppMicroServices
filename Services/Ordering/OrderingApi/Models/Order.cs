using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrderingApi.Models;

public class Order
{
    [Key]
    public int Id { get; set; }
    public string? UserId { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string OrderStatus { get; set; } = "New Order";
    public double TotalPrice { get; set; }
    public Address? DeliveryAddress { get; set; }
    public List<BasketItem>? BasketItems { get; set; }
}
