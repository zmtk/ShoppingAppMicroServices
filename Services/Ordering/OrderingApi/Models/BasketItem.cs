using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OrderingApi.Models;

public class BasketItem
{
    [Key]
    [JsonIgnore]
    public int BasketItemId { get; set; }
    public int ProductId { get; set; }
    public string? Name { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
            
    [NotMapped]
    public double Total { get { return Price * Quantity; } }

    [JsonIgnore]
    public Order? Order { get; set; }
}
