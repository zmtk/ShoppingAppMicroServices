namespace CatalogApi.Dtos;

public class UpdateBasketPriceDto
{
    public int Id { get; set; }
    public double NewPrice { get; set; }
    public string? Event { get; set; } = "Event_Update_Basket_Prices";
    
}