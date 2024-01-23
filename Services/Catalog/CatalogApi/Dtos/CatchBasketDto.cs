namespace CatalogApi.Dtos;

 public class CatchBasketDto
    {
        public int ProductId { get; set; }
        public string? BasketEvent { get; set; }
        public int Quantity { get; set; } = 1;
    
    }