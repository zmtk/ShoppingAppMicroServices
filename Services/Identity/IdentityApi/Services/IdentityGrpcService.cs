using Auth;
using Grpc.Core;
using IdentityApi.Data;
using IdentityApi.Models;

namespace IdentityApi.Services;

public class IdentityGrpcService : IdentityGrpc.IdentityGrpcBase
{
    private readonly IAddressRepository _addressRepo;

    public IdentityGrpcService(IAddressRepository addressRepo)
    {
        _addressRepo = addressRepo;
    }
    private string GetAuthenticatedUserId(ServerCallContext callContext)
    {
        var authorizationKey = "authorization";
        var accessToken = callContext.RequestHeaders?.FirstOrDefault(header => header.Key == authorizationKey)?.Value;
        bool accessTokenExist = !string.IsNullOrEmpty(accessToken);

        if (!accessTokenExist)
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User not logged in."));

        return Authorize.GetUserId(accessToken);
    }


    public override async Task<GetAddressResponse> GetAddress(GetAddressRequest request, ServerCallContext callContext)
    {
        if (request.AddressId <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid AddressId"));

        string uid = GetAuthenticatedUserId(callContext);

        Address address = _addressRepo.GetUserAddressById(int.Parse(uid), request.AddressId);


        Console.WriteLine("uid" + uid);
        return await Task.FromResult(new GetAddressResponse
        {
            Id = address.Id,
            UserId = address.UserId,
            FirstName = address.FirstName ?? "",  // Use empty string if nullable
            LastName = address.LastName ?? "",    // Use empty string if nullable
            PhoneNumber = address.PhoneNumber ?? "", // Use empty string if nullable
            City = address.City ?? "",             // Use empty string if nullable
            District = address.District ?? "",     // Use empty string if nullable
            Neighborhood = address.Neighborhood ?? "", // Use empty string if nullable
            StreetAddress = address.StreetAddress ?? "", // Use empty string if nullable
            AddressType = address.AddressType ?? "", // Use empty string if nullable
        });
    }
}