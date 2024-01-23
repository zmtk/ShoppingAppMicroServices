using Auth;
using Grpc.Core;
using OrderingApi.Data;
using OrderingApi.Models;
using Google.Protobuf.WellKnownTypes;

namespace OrderingApi.Services;

public class OrderingGrpcService : OrderingGrpc.OrderingGrpcBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IGrpcClient _grpcClient;

    public OrderingGrpcService(IOrderRepository orderRepository, IGrpcClient grpcClient)
    {
        _orderRepository = orderRepository;
        _grpcClient = grpcClient;
    }

    public override async Task<GetOrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext callContext)
    {
        if (request.AddressId <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid AddressId"));

        string? uid = GetAuthenticatedUserId(callContext);
        string? accessToken = GetAccessToken(callContext);


        Address? deliveryAddress = _grpcClient.GetDeliveryAddress(accessToken, request.AddressId);

        if (deliveryAddress == null)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "No address found with provided addressId"));


        var basket = _grpcClient.GetBasket(accessToken);

        if (basket == null)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "No address found with provided addressId"));

        Order order = _orderRepository.CreateOrder(new Order
        {
            UserId = uid,
            TotalPrice = basket.Value.BasketTotal,
            BasketItems = basket.Value.BasketItems,
            DeliveryAddress = deliveryAddress,
        });

        if (order == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Could not create order."));

        GetOrderResponse response = CreateGetOrderResponse(order);

        //if order created? check somehow?


        if(_grpcClient.EmptyBasket(accessToken))
            Console.WriteLine("Basket Emptied");

            
        return await Task.FromResult(response);
    }
    

    public override async Task<GetUserOrdersResponse> GetUserOrders(GetUserOrdersRequest request, ServerCallContext callContext)
    {
        string? uid = GetAuthenticatedUserId(callContext);

        GetUserOrdersResponse response = new GetUserOrdersResponse();
        List<Order> orders = await _orderRepository.GetUserOrdersAsync(uid!);

        foreach (Order order in orders)
        {
            response.Orders.Add(CreateGetOrderResponse(order));
        }

        Console.WriteLine($"here i am -> UID = {uid}");
        return await Task.FromResult(response);
    }

    public override async Task<GetOrderResponse> GetOrder(GetOrderRequest request, ServerCallContext callContext)
    {

        if (request.Id <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid ProductId"));

        string? uid = GetAuthenticatedUserId(callContext);

        Order? order = await _orderRepository.GetOrderAsync(request.Id, uid!);

        if (order == null)
            throw new RpcException(new Status(StatusCode.NotFound, "No order found with provided Id"));

        GetOrderResponse response = CreateGetOrderResponse(order);

        return await Task.FromResult(response);

    }

    private string? GetAccessToken(ServerCallContext callContext)
    {
        var authorizationKey = "authorization";
        var accessToken = callContext.RequestHeaders?.FirstOrDefault(header => header.Key == authorizationKey)?.Value;
        return accessToken;
    }

    private string? GetAuthenticatedUserId(ServerCallContext callContext)
    {

        var accessToken = GetAccessToken(callContext);
        bool accessTokenExist = !string.IsNullOrEmpty(accessToken);
        string? uid = Authorize.GetUserId(accessToken);

        if (!accessTokenExist || uid == null)
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User not logged in."));

        return uid;
    }

    private GetOrderResponse CreateGetOrderResponse(Order order)
    {
        GetOrderResponse response = new GetOrderResponse
        {
            Id = order.Id,
            UserId = order.UserId,
            Date = Timestamp.FromDateTime(order.Date),
            OrderStatus = order.OrderStatus,
            TotalPrice = order.TotalPrice,
            DeliveryAddress = order.DeliveryAddress != null
            ? new AddressResponse
            {
                FirstName = order.DeliveryAddress.FirstName,
                LastName = order.DeliveryAddress.LastName,
                PhoneNumber = order.DeliveryAddress.PhoneNumber,
                City = order.DeliveryAddress.City,
                District = order.DeliveryAddress.District,
                Neighborhood = order.DeliveryAddress.Neighborhood,
                StreetAddress = order.DeliveryAddress.StreetAddress,
                AddressType = order.DeliveryAddress.AddressType
            }
            : null
        };

        foreach (var basketItem in order.BasketItems ?? Enumerable.Empty<BasketItem>())
        {
            response.BasketItems.Add(new BasketItemResponse
            {
                ProductId = basketItem.ProductId,
                Name = basketItem.Name,
                Price = basketItem.Price,
                Quantity = basketItem.Quantity,
                Total = basketItem.Total
            });
        }

        return response;
    }
}