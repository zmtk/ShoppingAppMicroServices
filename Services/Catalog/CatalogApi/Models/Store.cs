using System.ComponentModel.DataAnnotations;

namespace CatalogApi.Models;

// public class Store
// {
//     [Key, Required]
//     public int Id { get; set; }

//     [Required]
//     public string? Name { get; set; }

//     public List<Employee> Employees {get; set;} = new List<Employee>();
// }

public class Store
    {
        [Key, Required]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        // Generic collection to hold employees
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public bool Inactive { get; set; } = false;

    }