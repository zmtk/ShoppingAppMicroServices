namespace BasketApi.Models;

public class BasketItem
{

    public int ProductId { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; } = 1;
    public bool Inactive { get; set; } = false;
    public double Total { get { return Price * Quantity; } }



}
