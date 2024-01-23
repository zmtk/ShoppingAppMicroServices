using Microsoft.EntityFrameworkCore;
using CatalogApi.Models;

namespace CatalogApi.Data;
public static class PrepDb
{
    public static void PopulateDb(IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetService<AppDbContext>();

            if (context != null)
            {
                SeedData(context);
            }
        }
    }

    private static void MigrateDb(AppDbContext context)
    {
        try
        {
            context.Database.Migrate();
            Console.WriteLine("--> Applied migrations...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not run migrations: {ex.Message}");
        }
    }

    private static void SeedData(AppDbContext context)
    {

        MigrateDb(context);

        int hStoreId; // Variable to store the Id of the "HStore"
        if (!context.Stores.Any())
        {
            Console.WriteLine("--> Seeding Stores Data");

            Employee employee = new()
            {
                UserId = 1,
                Group = 1100
            };

            Store store = new()
            {
                Name = "HStore",
            };

            store.Employees.Add(employee);

            context.Stores.Add(store);
            context.SaveChanges();
            hStoreId = store.Id;
        }
        else
        {
            Console.WriteLine("--> App already have Stores data");
            hStoreId = context.Stores.First().Id;
        }

        if (!context.Products.Any())
        {
            Console.WriteLine("--> Seeding Products Data...");

            context.Products.AddRange(
                new Product
                {
                    Name = "SuperShoes Air",
                    Type = "Shoes",
                    Brand = "Adidas",
                    Gender = "Male",
                    Price = 5.99,
                    StoreId = hStoreId
                },
                new Product
                {
                    Name = "Pro Runners max",
                    Type = "Shoes",
                    Brand = "Nike",
                    Gender = "Male",
                    Price = 9.99,
                    StoreId = hStoreId
                },
                new Product
                {
                    Name = "MemoryFoam",
                    Type = "Shoes",
                    Brand = "Sketchers",
                    Gender = "Male",
                    Price = 4.99,
                    StoreId = hStoreId
                },
                new Product
                {
                    Name = "Converse",
                    Type = "Shoes",
                    Brand = "Converse",
                    Gender = "Male",
                    Price = 7.99,
                    StoreId = hStoreId
                },
                new Product
                {
                    Name = "SuperShoes Air 2",
                    Type = "Shoes",
                    Brand = "Adidas",
                    Gender = "Male",
                    Price = 7.99,
                    StoreId = hStoreId
                },
                new Product
                {
                    Name = "SuperShoes Air",
                    Type = "Shoes",
                    Brand = "Adidas",
                    Gender = "Female",
                    Price = 5.99,
                    StoreId = hStoreId
                },
                new Product
                {
                    Name = "Pro Runners max",
                    Type = "Shoes",
                    Brand = "Nike",
                    Gender = "Female",
                    Price = 9.99,
                    StoreId = hStoreId
                },
                new Product
                {
                    Name = "MemoryFoam",
                    Type = "Shoes",
                    Brand = "Sketchers",
                    Gender = "Female",
                    Price = 4.99,
                    StoreId = hStoreId
                },
                new Product
                {
                    Name = "Converse",
                    Type = "Shoes",
                    Brand = "Converse",
                    Gender = "Female",
                    Price = 7.99,
                    StoreId = hStoreId
                },
                new Product
                {
                    Name = "SuperShoes Air 2",
                    Type = "Shoes",
                    Brand = "Adidas",
                    Gender = "Female",
                    Price = 7.99,
                    StoreId = hStoreId
                }
            );
            context.SaveChanges();
        }
        else
        {
            Console.WriteLine("--> App already have Products data");
        }
    }
}
