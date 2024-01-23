using OrderingApi.Models;

namespace OrderingApi.Data;

public interface IOrderRepository
{
    Order CreateOrder(Order order);
    IEnumerable<Order> GetAllOrders();
    Task<Order?> GetOrderAsync(int orderId, string userId);
    Task<List<Order>> GetUserOrdersAsync(string userId);


}