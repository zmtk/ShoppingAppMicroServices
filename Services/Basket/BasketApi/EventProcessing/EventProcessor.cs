using System.Text.Json;
using BasketApi.Data;
using BasketApi.Dtos;

namespace BasketApi.EventrProcessing;

enum EventType
{
    UpdateBasketPrices,
    EnableProduct,
    DisableProduct,
    Undetermined
}

public class EventProcessor(IServiceScopeFactory scopeFactory) : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    public async Task ProcessEventAsync(string message)
    {
        var eventType = DetermineEvent(message);

        switch (eventType)
        {
            case EventType.UpdateBasketPrices:
                await UpdateBasketPrices(message);
                break;
            case EventType.EnableProduct:
                await ToggleProduct(message, active:true);
                break;
            case EventType.DisableProduct:
                await ToggleProduct(message, active:false);
                break;
            default:
                break;
        }
    }

    private async Task ToggleProduct(string message, bool active)
    {
        var deleteProductDto = JsonSerializer.Deserialize<DeleteProductDto>(message);
        if(deleteProductDto != null)
        {

            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IBasketRepo>();

                try
                {
                    await repo.ToggleProduct(deleteProductDto, active);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not update prices: {ex.Message}");
                }
            }

        }

    }
    private async Task UpdateBasketPrices(string message)
    {
        var updateBasketPriceDto = JsonSerializer.Deserialize<UpdateBasketPriceDto>(message);

        if (updateBasketPriceDto != null)
        {
            Console.WriteLine($"--> Updating ProductID:{updateBasketPriceDto.Id} NewPrice:{updateBasketPriceDto.NewPrice}");

            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IBasketRepo>();

                try
                {
                    await repo.UpdateBasketPriceAsync(updateBasketPriceDto);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not update prices: {ex.Message}");
                }
            }
        }
    }

    private static EventType DetermineEvent(string notificationMessage)
    {
        Console.Write("--> Determining Event: ");

        // Deserialize JSON string to GenericEventDto
        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

        // Check if deserialization was successful
        if (eventType != null)
        {
            switch (eventType.Event)
            {
                case "Event_Update_Basket_Prices":
                    Console.WriteLine("--> Update basket prices event detected");
                    return EventType.UpdateBasketPrices;
                // case "Event_Delete_Product_From_Basket":
                //     Console.WriteLine("--> Delete product from basket event detected");
                //     return EventType.DeleteProduct;
                case "Event_Disable_Product_From_Basket":
                    Console.WriteLine("--> Disable product from basket event detected");
                    return EventType.DisableProduct;
                case "Event_Enable_Product_From_Basket":
                    Console.WriteLine("--> Enable product from basket event detected");
                    return EventType.EnableProduct;
                default:
                    Console.WriteLine("--> Could not determine the event type");
                    return EventType.Undetermined;
            }
        }
        else
        {
            Console.WriteLine("--> Failed to deserialize the event. The input message may be in an unexpected format.");
            return EventType.Undetermined;
        }
    }

}