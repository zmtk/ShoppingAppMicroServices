using Microsoft.EntityFrameworkCore;
using OrderingApi.Models;

namespace OrderingApi.Data;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public Order CreateOrder(Order order)
    {
        _context.Orders.Add(order);
        _context.SaveChanges();
        return order;
    }



    public IEnumerable<Order> GetAllOrders()
    {
        return _context.Orders
            .Include(o => o.BasketItems) // Include BasketItems in the query
            .Include(o => o.DeliveryAddress) // Include DeliveryAddress in the query
            .ToList();
    }

    public async Task<Order?> GetOrderAsync(int orderId, string userId)
    {
        return await _context.Orders
                        .Include(o => o.BasketItems) // Include BasketItems in the query
                        .Include(o => o.DeliveryAddress) // Include DeliveryAddress in the query
                        .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
    }

    public async Task<List<Order>> GetUserOrdersAsync(string userId)
    {
        return await _context.Orders
                        .Include(o => o.BasketItems) // Include BasketItems in the query
                        .Include(o => o.DeliveryAddress) // Include DeliveryAddress in the query
                        .Where(o => o.UserId == userId)
                        .ToListAsync();
    }

}