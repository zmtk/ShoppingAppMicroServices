using OrderingApi.Models;

namespace OrderingApi.Services;

public interface IGrpcClient {
    (List<BasketItem> BasketItems, double BasketTotal)? GetBasket(string? accessToken);
    Address? GetDeliveryAddress(string? accessToken, int addressId);
    bool EmptyBasket(string accessToken);


}