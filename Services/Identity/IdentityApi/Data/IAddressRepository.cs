using IdentityApi.Models;

namespace IdentityApi.Data;

public interface IAddressRepository
{
    Address Create(Address address);
    IEnumerable<Address> GetUserAddresses(int userId);
    Address GetUserAddressById(int userId, int? addressId);
    Address UpdateAddress(int userId, int addressId, Address updatedAddress);

    void RemoveAdress(int userId, int? addressId);


}