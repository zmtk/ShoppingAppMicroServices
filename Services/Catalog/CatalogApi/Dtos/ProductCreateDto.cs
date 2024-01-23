using System.ComponentModel.DataAnnotations;

namespace CatalogApi.Dtos;

public class ProductCreateDto
{
    [Required]
    public string? Name { get; set; }
    
    [Required]
    public string? Type { get; set; }
    
    [Required]
    public string? Brand { get; set; }
    
    [Required]
    public string? Gender { get; set; }
    
    [Required]
    public double Price { get; set; }

    [Required]
    public int StoreId { get; set; }

}