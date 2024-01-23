using BasketApi;
using Grpc.Net.Client;
using Grpc.Core;
using OrderingApi.Models;
using IdentityApi;

namespace OrderingApi.Services;

public class GrpcClient : IGrpcClient
{
    private readonly IConfiguration _configuration;

    public GrpcClient(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Address? GetDeliveryAddress(string? accessToken, int addressId)
    {
        if (accessToken == null || _configuration["GrpcIdentityApi"] == null)
            return null;

        Console.WriteLine($"--> Calling GRPC Service {_configuration["GrpcIdentityApi"]}");

        var channel = GrpcChannel.ForAddress(_configuration["GrpcIdentityApi"]!);
        var client = new IdentityGrpc.IdentityGrpcClient(channel);
        var metadata = new Metadata { new Metadata.Entry("Authorization", accessToken) };
        var request = new GetAddressRequest { AddressId = addressId };

        try
        {
            var reply = client.GetAddress(request, headers: metadata);

            Address deliveryAddress = new Address
            {
                FirstName = reply.FirstName,
                LastName = reply.LastName,
                PhoneNumber = reply.PhoneNumber,
                City = reply.City,
                District = reply.District,
                Neighborhood = reply.Neighborhood,
                StreetAddress = reply.StreetAddress,
                AddressType = reply.AddressType
            };

            return deliveryAddress;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Couldnot call GRPC Server {ex.Message}");
        }

        return null;
    }

    public (List<BasketItem> BasketItems, double BasketTotal)? GetBasket(string? accessToken)
    {
        if (accessToken == null || _configuration["GrpcBasketApi"] == null)
            return null;

        Console.WriteLine($"--> Calling GRPC Service {_configuration["GrpcBasketApi"]}");

        var channel = GrpcChannel.ForAddress(_configuration["GrpcBasketApi"]!);
        var client = new BasketGrpc.BasketGrpcClient(channel);
        var request = new GetBasketRequest();
        var metadata = new Metadata { new Metadata.Entry("Authorization", accessToken!) };

        try
        {
            var reply = client.GetBasket(request, headers: metadata);

            List<BasketItem> basketItems = reply.BasketItems
                    .Where(item => !item.Inactive)
                    .Select(item => new BasketItem
                    {
                        ProductId = item.ProductId,
                        Name = item.Name,
                        Price = item.Price,
                        Quantity = item.Quantity,
                    }).ToList();

            double BasketTotal = reply.BasketTotal;

            return (basketItems, BasketTotal);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Couldnot call GRPC Server {ex.Message}");
        }

        return null;
    }

    public bool EmptyBasket(string accessToken)
    {
        if (accessToken == null || _configuration["GrpcBasketApi"] == null)
            return false;

        Console.WriteLine($"--> Calling GRPC Service {_configuration["GrpcBasketApi"]}");

        var channel = GrpcChannel.ForAddress(_configuration["GrpcBasketApi"]!);
        var client = new BasketGrpc.BasketGrpcClient(channel);
        var request = new EmptyBasketRequest();
        var metadata = new Metadata { new Metadata.Entry("Authorization", accessToken!) };

        try
        {
            var reply = client.EmptyBasket(request, headers: metadata);

            return reply.Completed;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Couldnot call GRPC Server {ex.Message}");
        }

        return false;
    }

}