using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CatalogApi.Models;

// public class Employee
// {
//     [Key]
//     [Required]
//     public int Id { get; set; }
//     public int UserId { get; set; }
//     // 1000 = Employee, 1050 = Manager, 1100 = Owner
//     public int Group { get; set; } = 1000;

// }

public class Employee
{
    [Key]
    [Required]
    [JsonIgnore]
    public int Id { get; set; }

    public int UserId { get; set; }

    // 1000 = Employee, 1050 = Manager, 1100 = Owner
    public int Group { get; set; } = 1000;

    [JsonIgnore]
    // Foreign key property
    public int StoreId { get; set; }

    // Navigation property
    [JsonIgnore]
    public Store? Store { get; set; }
}