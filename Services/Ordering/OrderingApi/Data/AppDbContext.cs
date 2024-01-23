using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OrderingApi.Models;

namespace OrderingApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opt): base(opt)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<BasketItem> BasketItems { get; set; }
    public DbSet<Address> Address { get; set; }

}