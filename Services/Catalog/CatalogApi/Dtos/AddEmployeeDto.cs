using System.ComponentModel.DataAnnotations;

namespace CatalogApi.Dtos;

public class AddEmployeeDto
{
    [Required]
    public int StoreId { get; set; }
    [Required]
    public int UserId { get; set; }
    public int Group { get; set; } = 1000;

}