namespace IdentityApi.Dtos;

public class AddressDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? Neighborhood { get; set; }
    public string? StreetAddress { get; set; }
    public string? AddressType { get; set; }

}