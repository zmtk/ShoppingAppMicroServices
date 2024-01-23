using System.ComponentModel.DataAnnotations;

namespace BasketApi.Models;

public class Basket {

    [Required]
    public string UserId { get; set; } = $"basket:{Guid.NewGuid()}";
    
    public ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();

    public double BasketTotal {
        get
        {
            double total = 0;
            foreach(BasketItem basketItem in BasketItems)
            {
                if(basketItem.Inactive == false)
                    total += basketItem.Total;
            }
            return total;
        }
    }
}