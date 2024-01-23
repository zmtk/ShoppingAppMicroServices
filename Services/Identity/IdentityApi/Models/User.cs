using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace IdentityApi.Models;

public class User
{
    [Key]
    [Required]
    public int Id { get; set; }
    
    [Required]
    public string FirstName { get; set; }
    
    [Required]
    public string LastName { get; set; }

    [Required]
    public string Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? DateOfBirth { get; set; }

    [Required]
    public string Roles { get; set; } = "user";


    [JsonIgnore]
    public string RefreshToken { get; set; } = "";

    [JsonIgnore]
    [Required]
    public string Password { get; set; }
}