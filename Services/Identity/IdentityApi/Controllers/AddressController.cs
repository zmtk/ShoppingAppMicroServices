using Auth;
using IdentityApi.Data;
using IdentityApi.Dtos;
using IdentityApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace IdentityApi.Controllers;

[Route("api/identity/[controller]")]
[ApiController]
public class AddressController : Controller
{
    private readonly IAddressRepository _addressRepository;

    public AddressController(IAddressRepository addressRepository)
    {
        _addressRepository = addressRepository;
    }

    [HttpPost]
    public IActionResult AddAddress(AddressDto addressDto)
    {

        try
        {
            var accessToken = Request.Headers.Authorization;
            bool accessTokenExist = !string.IsNullOrWhiteSpace(accessToken);

            string? uid = accessTokenExist ? Authorize.GetUserId(accessToken) : null;

            if (uid == null)
                throw new SecurityTokenExpiredException();

            Address address = new Address
            {
                UserId = int.Parse(uid),
                FirstName = addressDto.FirstName,
                LastName = addressDto.LastName,
                PhoneNumber = addressDto.PhoneNumber,
                City = addressDto.City,
                District = addressDto.District,
                Neighborhood = addressDto.Neighborhood,
                StreetAddress = addressDto.StreetAddress,
                AddressType = addressDto.AddressType
            };

            return Ok(_addressRepository.Create(address));

        }
        catch (SecurityTokenExpiredException _)
        {
            return Unauthorized(new { message = "Access token is expired" });
            // return RefreshAccessToken();
        }

    }

    [HttpGet()]
    public IActionResult GetUserAddresses()
    {
        try
        {
            var accessToken = Request.Headers.Authorization;
            bool accessTokenExist = !string.IsNullOrWhiteSpace(accessToken);

            string? uid = accessTokenExist ? Authorize.GetUserId(accessToken) : null;

            if (uid == null)
                throw new SecurityTokenExpiredException();

            // var user = _userRepository.GetUserById(uid);


            return Ok(_addressRepository.GetUserAddresses(int.Parse(uid)));

        }
        catch (SecurityTokenExpiredException _)
        {
            return Unauthorized(new { message = "Access token is expired" });
            // return RefreshAccessToken();
        }

    }

    [HttpGet("GetAdress")]
    public IActionResult GetUserAddressById(GetAddressDto getAddressDto)
    {

        try
        {
            var accessToken = Request.Headers.Authorization;
            bool accessTokenExist = !string.IsNullOrWhiteSpace(accessToken);

            string? uid = accessTokenExist ? Authorize.GetUserId(accessToken) : null;

            if (uid == null)
                throw new SecurityTokenExpiredException();

            Console.WriteLine("this"+uid);
            // var user = _userRepository.GetUserById(uid);

            return Ok(_addressRepository.GetUserAddressById(int.Parse(uid), getAddressDto.Id));

        }
        catch (SecurityTokenExpiredException _)
        {
            return Unauthorized(new { message = "Access token is expired" });
            // return RefreshAccessToken();
        }
    }

    [HttpPost("UpdateAddress")]
    public IActionResult UpdateAddress(AddressDto addressDto)
    {
        try
        {
            var accessToken = Request.Headers.Authorization;
            bool accessTokenExist = !string.IsNullOrWhiteSpace(accessToken);
            string? uid = accessTokenExist ? Authorize.GetUserId(accessToken) : null;

            if (uid == null)
                throw new SecurityTokenExpiredException();

            var address = new Address
            {
                FirstName = addressDto.FirstName,
                LastName = addressDto.LastName,
                PhoneNumber = addressDto.PhoneNumber,
                City = addressDto.City,
                District = addressDto.District,
                Neighborhood = addressDto.Neighborhood,
                StreetAddress = addressDto.StreetAddress,
                AddressType = addressDto.AddressType
            };

            var updatedAddress = _addressRepository.UpdateAddress(int.Parse(uid), addressDto.Id, address);

            return Ok(updatedAddress);
        }
        catch (SecurityTokenExpiredException _)
        {
            return Unauthorized(new { message = "Access token is expired" });
        }
        catch (KeyNotFoundException _)
        {
            return NotFound(new { message = "Address Not Found." });
        }
    }

    [HttpDelete("RemoveAddress")]
    public IActionResult RemoveAdress(GetAddressDto getAddressDto)
    {
        try
        {
            var accessToken = Request.Headers.Authorization;
            bool accessTokenExist = !string.IsNullOrWhiteSpace(accessToken);
            string? uid = accessTokenExist ? Authorize.GetUserId(accessToken) : null;

            if (uid == null)
                throw new SecurityTokenExpiredException();

            _addressRepository.RemoveAdress(int.Parse(uid), getAddressDto.Id);
            return Ok(new {message= "Address successfully removed"});
        }
        catch (SecurityTokenExpiredException _)
        {
            return Unauthorized(new { message = "Access token is expired" });
        }
        catch (KeyNotFoundException _)
        {
            return NotFound(new { message = "Address Not Found." });
        }
    }

}
