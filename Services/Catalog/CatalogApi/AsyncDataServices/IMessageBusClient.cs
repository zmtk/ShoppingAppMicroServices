using CatalogApi.Dtos;

namespace CatalogApi.AsyncDataServices;

public interface IMessageBusClient 
{

    void UpdateBasketPrices(UpdateBasketPriceDto updateBasketPricesDto);
    void ToggleProductFromBasket(ToggleProductFromBasketDto toggleProductFromBasketDto);
    
}