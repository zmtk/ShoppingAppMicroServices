namespace CatalogApi.Dtos
{

    public class UpdateBasketDto
    {
        public string? UserId { get; set; }
        public int? ProductId { get; set; }
        public string? Name { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
        public string? Event { get; set; }
    }
}