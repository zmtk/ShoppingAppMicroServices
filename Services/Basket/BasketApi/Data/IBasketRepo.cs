using BasketApi.Dtos;
using BasketApi.Models;

namespace BasketApi.Data;

public interface IBasketRepo 
{
    Task UpdateBasketAsync(Basket basket);
    Task<Basket?> GetBasketByIdAsync(string userId);
    Task<bool> ProductExistAsync(string userId, int productId);
    Task RemoveBasketAsync(string userId);
    IAsyncEnumerable<Basket> GetAllBasketsAsync();
    Task<double> RemoveItemAsync(string userId, int productId);
    Task<Basket?> SetQuantityAsync(string userId, int productId, int quantity);
    Task ReduceQuantityAsync(string userId, int productId, int reduceValue = 1);
    Task IncreaseQuantityAsync(string userId, int productId, int increaseValue = 1);
    Task AddToBasketAsync(string userId, BasketItem basketItem);
    Task<bool> UpdateBasketPriceAsync(UpdateBasketPriceDto updateBasketPriceDto);
    Task<bool> ToggleProduct(DeleteProductDto deleteProductDto, bool active);


}