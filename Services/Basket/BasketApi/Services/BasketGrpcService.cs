using Auth;
using AutoMapper;
using BasketApi.Data;
using BasketApi.Models;
using Grpc.Core;

namespace BasketApi.Services;

public class BasketGrpcService(IBasketRepo repo, IMapper mapper) : BasketGrpc.BasketGrpcBase
{
    private readonly IBasketRepo _repo = repo;
    private readonly IMapper _mapper = mapper;

    private static string GetAuthenticatedUserId(ServerCallContext callContext)
    {
        const string AuthorizationKey = "authorization";

        var accessToken = callContext.RequestHeaders?.FirstOrDefault(header => header.Key == AuthorizationKey)?.Value;

        if (string.IsNullOrEmpty(accessToken))
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User not logged in."));

        string? userId = Authorize.GetUserId(accessToken);

        if (string.IsNullOrEmpty(userId))
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User not logged in."));

        return userId!;
    }


    public override async Task<AddToBasketResponse> AddToBasket(AddToBasketRequest request, ServerCallContext callContext)
    {

        if (request.UserId <= 0 || request.ProductId <= 0 || request.Name == string.Empty || request.Price <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Data Corrupted."));

        var basketItem = _mapper.Map<BasketItem>(request);

        await _repo.AddToBasketAsync(request.UserId.ToString(), basketItem);

        return await Task.FromResult(new AddToBasketResponse
        {
            UserId = request.UserId,
            Message = $"Product:{request.ProductId} is Added to user basket "
        });

    }

    public override async Task<SetProductQuantityResponse> SetProductQuantity(SetProductQuantityRequest request, ServerCallContext callContext)
    {
        if (request.ProductId <= 0 || request.Quantity < 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid ProductId and Quantity"));

        string uid = GetAuthenticatedUserId(callContext);

        if (!await _repo.ProductExistAsync(uid, request.ProductId))
            throw new RpcException(new Status(StatusCode.NotFound, "Product not found in the basket"));

        if (request.Quantity == 0)
        {
            return await Task.FromResult(new SetProductQuantityResponse
            {
                UpdatedTotal = await _repo.RemoveItemAsync(uid, request.ProductId)
            });
        }

        Basket? updatedBasket = await _repo.SetQuantityAsync(uid, request.ProductId, request.Quantity) 
            ?? throw new RpcException(new Status(StatusCode.Internal, "Failed to update the basket."));
        BasketItem? updatedItem = updatedBasket.BasketItems.FirstOrDefault(item => item.ProductId == request.ProductId) 
            ?? throw new RpcException(new Status(StatusCode.Internal, "Failed to update the item in the basket."));

        return await Task.FromResult(new SetProductQuantityResponse
        {
            UpdatedQuantity = updatedItem.Quantity,
            UpdatedPrice = updatedItem.Total,
            UpdatedTotal = updatedBasket.BasketTotal
        });
    }

    public override async Task<DeleteProductResponse> DeleteProduct(DeleteProductRequest request, ServerCallContext callContext)
    {
        if (request.Id <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid ProductId"));

        string uid = GetAuthenticatedUserId(callContext);

        return await Task.FromResult(new DeleteProductResponse
        {
            UpdatedTotal = await _repo.RemoveItemAsync(uid, request.Id)
        });
    }

    public override async Task<GetBasketResponse> GetBasket(GetBasketRequest request, ServerCallContext callContext)
    {
        string uid = GetAuthenticatedUserId(callContext);

        var basket = await _repo.GetBasketByIdAsync(uid) ?? throw new RpcException(new Status(StatusCode.NotFound, "User Doesn't have a basket."));

        // Use AutoMapper to map Basket to GetBasketResponse
        GetBasketResponse response = _mapper.Map<GetBasketResponse>(basket);

        return await Task.FromResult(response);
    }


    public override async Task<EmptyBasketResponse> EmptyBasket(EmptyBasketRequest request, ServerCallContext callContext)
    {
        string uid = GetAuthenticatedUserId(callContext);

        EmptyBasketResponse response = new();

        try
        {
            await _repo.RemoveBasketAsync(uid);
            response.Completed = true;
        }
        catch
        {
            response.Completed = false;
        }

        return await Task.FromResult(response);
    }


}