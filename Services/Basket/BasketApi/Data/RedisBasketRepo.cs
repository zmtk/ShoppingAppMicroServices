using System.Text.Json;
using BasketApi.Dtos;
using BasketApi.Models;
using StackExchange.Redis;

namespace BasketApi.Data;

public class RedisBasketRepo(IConnectionMultiplexer redis) : IBasketRepo
{
    private readonly IConnectionMultiplexer _redis = redis;

    public async Task UpdateBasketAsync(Basket basket)
    {
        if (basket == null)
        {
            throw new ArgumentNullException(nameof(basket));
        }

        var db = _redis.GetDatabase();
        var serialBasket = JsonSerializer.Serialize(basket);

        await db.StringSetAsync(basket.UserId, serialBasket);
    }


    public async IAsyncEnumerable<Basket> GetAllBasketsAsync()
    {
        var db = _redis.GetDatabase();

        // Assuming baskets are stored with keys that have a common prefix, like "Basket:"
        var server = db.Multiplexer.GetServer(_redis.GetEndPoints().First());

        await foreach (var key in server.KeysAsync())
        {
            var basket = await db.StringGetAsync(key);

            if (!string.IsNullOrEmpty(basket))
            {
                yield return JsonSerializer.Deserialize<Basket>(basket);
            }
        }
    }


    public async Task<Basket?> GetBasketByIdAsync(string userId)
    {
        var db = _redis.GetDatabase();
        var basket = await db.StringGetAsync(userId);

        if (!string.IsNullOrEmpty(basket))
        {
            return JsonSerializer.Deserialize<Basket>(basket);
        }

        return null;
    }


    public async Task<bool> ProductExistAsync(string userId, int productId)
    {
        var basket = await GetBasketByIdAsync(userId);
        var basketItems = basket?.BasketItems;

        return basketItems?.Any(item => item.ProductId == productId) ?? false;
    }


    public async Task RemoveBasketAsync(string userId)
    {
        var basket = await GetBasketByIdAsync(userId);

        if (basket != null)
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(userId);
        }
    }

    public async Task<double> RemoveItemAsync(string userId, int productId)
    {
        var basket = await GetBasketByIdAsync(userId);

        if (basket != null)
        {
            var itemToRemove = basket.BasketItems.FirstOrDefault(item => item.ProductId == productId);

            if (itemToRemove != null)
            {
                basket.BasketItems.Remove(itemToRemove);
                await UpdateBasketAsync(basket);
                return basket.BasketTotal;
            }
        }

        return 0;
    }


    public async Task<Basket?> SetQuantityAsync(string userId, int productId, int quantity)
    {
        var basket = await GetBasketByIdAsync(userId);

        if (basket != null)
        {
            var itemToModify = basket.BasketItems.FirstOrDefault(item => item.ProductId == productId);

            if (itemToModify != null)
            {
                Console.WriteLine("Increasing quantity");
                itemToModify.Quantity = quantity;
                await UpdateBasketAsync(basket);
                return basket;
            }
        }

        return null;
    }

    public async Task ReduceQuantityAsync(string userId, int productId, int reduceValue = 1)
    {
        var basket = await GetBasketByIdAsync(userId);

        if (basket != null)
        {
            foreach (var item in basket.BasketItems)
            {
                if (item.ProductId == productId)
                {
                    Console.WriteLine("Found item.");

                    if (item.Quantity - reduceValue > 0)
                    {
                        Console.WriteLine("Reducing quantity");
                        item.Quantity -= reduceValue;
                        await UpdateBasketAsync(basket);
                        break;
                    }
                    else
                    {
                        await RemoveItemAsync(userId, item.ProductId);
                        break;
                    }
                }
            }
        }
    }

    public async Task IncreaseQuantityAsync(string userId, int productId, int increaseValue = 1)
    {
        var basket = await GetBasketByIdAsync(userId);

        if (basket != null)
        {
            foreach (var item in basket.BasketItems)
            {
                if (item.ProductId == productId)
                {
                    Console.WriteLine("Increasing quantity");
                    item.Quantity += increaseValue;
                    await UpdateBasketAsync(basket);
                    break;
                }
            }
        }
    }


    public async Task AddToBasketAsync(string userId, BasketItem basketItem)
    {
        var basket = await GetBasketByIdAsync(userId);

        if (basket == null)
        {
            // User does not have a basket, create a new basket with the sent item
            await UpdateBasketAsync(new Basket { UserId = userId, BasketItems = { basketItem } });
        }
        else
        {
            var existingItem = basket.BasketItems.FirstOrDefault(item => item.ProductId == basketItem.ProductId);

            if (existingItem != null)
            {
                // Item already in the basket, increase quantity
                existingItem.Quantity += basketItem.Quantity;
            }
            else
            {
                // Item doesn't exist in the basket, add it
                basket.BasketItems.Add(basketItem);
            }

            // Update the basket
            await UpdateBasketAsync(basket);
        }
    }

    public async Task<bool> UpdateBasketPriceAsync(UpdateBasketPriceDto updateBasketPriceDto)
    {
        var baskets = new List<Basket>();

        await foreach (var basket in GetAllBasketsAsync())
        {
            baskets.Add(basket);
        }

        foreach (var basket in baskets)
        {
            // Check if BasketItem with the specified ProductId exists in the basket
            var basketItemToUpdate = basket.BasketItems.FirstOrDefault(item => item.ProductId == updateBasketPriceDto.Id);

            if (basketItemToUpdate != null)
            {
                // Update the Price with the new price
                basketItemToUpdate.Price = updateBasketPriceDto.NewPrice;

                // Save the updated basket back to the Redis database
                await UpdateBasketAsync(basket);

            }
        }

        return true; // Indicates that the update was successful
    }
    private static readonly ManualResetEvent DeleteProductEvent = new ManualResetEvent(true);

    public async Task<bool> ToggleProduct(DeleteProductDto deleteProductDto, bool active)
    {
        // Wait until the event is set (indicating that DeleteProduct is not running)
        DeleteProductEvent.WaitOne();

        // Set the event, indicating that DeleteProduct is now running
        DeleteProductEvent.Reset();

        try
        {
            var baskets = new List<Basket>();

            await foreach (var basket in GetAllBasketsAsync())
            {
                baskets.Add(basket);
            }

            foreach (var basket in baskets)
            {
                // Check if BasketItem with the specified ProductId exists in the basket
                var basketItemToRemove = basket.BasketItems.FirstOrDefault(item => item.ProductId == deleteProductDto.Id);

                if (basketItemToRemove != null)
                {

                    basketItemToRemove.Inactive = !active;
                    await UpdateBasketAsync(basket);

                }
            }

            return true; // Indicates that the update was successful
        }
        finally
        {
            // Set the event, indicating that DeleteProduct has finished running
            DeleteProductEvent.Set();
        }
    }







}