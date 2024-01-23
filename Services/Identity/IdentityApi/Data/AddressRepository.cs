using IdentityApi.Models;

namespace IdentityApi.Data;

public class AddressRepository : IAddressRepository
{

    private readonly AppDbContext _context;

    public AddressRepository(AppDbContext context)
    {
        _context = context;
    }

    public Address Create(Address address)
    {
        _context.Addresses.Add(address);
        _context.SaveChanges();

        return address;
    }

    public IEnumerable<Address> GetUserAddresses(int userId)
    {
        return _context.Addresses.Where(address => address.UserId == userId).ToList();
    }

    public Address GetUserAddressById(int userId, int? addressId)
    {
        if (addressId == null)
            return null;

        return _context.Addresses.FirstOrDefault(address => address.UserId == userId && address.Id == addressId);
    }

    public Address UpdateAddress(int userId, int addressId, Address updatedAddress)
    {
        Address address = GetUserAddressById(userId, addressId);

        if (address == null)
        {
            Console.WriteLine("i am here");
            throw new KeyNotFoundException();
        }

        address.FirstName = updatedAddress.FirstName ?? address.FirstName;
        address.LastName = updatedAddress.LastName ?? address.LastName;
        address.PhoneNumber = updatedAddress.PhoneNumber ?? address.PhoneNumber;
        address.City = updatedAddress.City ?? address.City;
        address.District = updatedAddress.District ?? address.District;
        address.Neighborhood = updatedAddress.Neighborhood ?? address.Neighborhood;
        address.StreetAddress = updatedAddress.StreetAddress ?? address.StreetAddress;
        address.AddressType = updatedAddress.AddressType ?? address.AddressType;

        _context.SaveChanges();

        return address;

    }

    public void RemoveAdress(int userId, int? addressId)
    {

        Address address = GetUserAddressById(userId, addressId);

        if (address == null)
        {
            throw new KeyNotFoundException();
        }

        _context.Addresses.Remove(address);
        _context.SaveChanges();

    }
}