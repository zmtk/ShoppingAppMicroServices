namespace CatalogApi.Dtos;

public class ProductReadDto
{ 
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Brand { get; set; }
    public string? Gender { get; set; }
    public double? OldPrice { get; set; }
    public double Price { get; set; }
    public int StoreId { get; set; }
    public bool Inactive { get; set; } = false;
}