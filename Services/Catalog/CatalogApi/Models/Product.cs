using System.ComponentModel.DataAnnotations;

namespace CatalogApi.Models;

public class Product
{
    [Key]
    [Required]
    public int Id { get; set; }
    [Required]
    public string? Name { get; set; }
    [Required]
    public string? Type { get; set; }
    [Required]
    public string? Brand { get; set; }
    [Required]
    public string? Gender { get; set; }
    public double? OldPrice { get; set; }
    [Required]
    public double Price { get; set; }
    [Required]
    public int StoreId { get; set; }
    public bool Inactive { get; set; } = false;
}