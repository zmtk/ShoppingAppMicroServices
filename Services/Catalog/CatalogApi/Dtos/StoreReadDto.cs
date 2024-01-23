using CatalogApi.Models;

namespace CatalogApi.Dtos;

public class StoreReadDto
{
    
    public int Id { get; set; }
    public string? Name { get; set; }
    public List<Employee>? Employees {get; set;}
    public bool Inactive { get; set; } = false;

}
