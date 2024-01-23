using System.ComponentModel.DataAnnotations;

namespace IdentityApi.Models;

public class Address {

    [Key]
    [Required]
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }

    [Required]
    public string? FirstName { get; set; }

    [Required]
    public string? LastName { get; set; }

    [Required]
    public string? PhoneNumber { get; set; }

    [Required]
    public string? City { get; set; }

    [Required]
    public string? District { get; set; }

    [Required]
    public string? Neighborhood { get; set; }

    [Required]
    public string? StreetAddress { get; set; }

    [Required]
    public string? AddressType { get; set; }


}