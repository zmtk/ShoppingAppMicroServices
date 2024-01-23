using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using OrderingApi.Models;

public class Address
{
    [Key, JsonIgnore]
    public int DeliveryAddressId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? Neighborhood { get; set; }
    public string? StreetAddress { get; set; }
    public string? AddressType { get; set; }

}