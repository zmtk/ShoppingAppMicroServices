namespace BasketApi.Dtos;

public class UpdateBasketPriceDto : GenericEventDto
{
    public int Id { get; set; }
    public double NewPrice { get; set; }
}