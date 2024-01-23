using IdentityApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityApi.Data;

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
            Console.WriteLine("--> Applied Migrations...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not run migrations: {ex.Message}");
        }
    }

    private static void SeedData(AppDbContext context)
    {
        MigrateDb(context);

        if (!context.Users.Any())
        {
            Console.WriteLine("--> Seeding Users Data");

            context.Users.AddRange(
                new User
                {
                    FirstName = "admin",
                    LastName = "a",
                    Email = "admin@a.com",
                    Roles = "admin,editor,user",
                    Password = BCrypt.Net.BCrypt.HashPassword("a")
                },
                new User
                {
                    FirstName = "editor",
                    LastName = "a",
                    Email = "editor@a.com",
                    Roles = "editor,user",
                    Password = BCrypt.Net.BCrypt.HashPassword("a")
                },
                new User
                {
                    FirstName = "user",
                    LastName = "a",
                    Email = "user@a.com",
                    Roles = "user",
                    Password = BCrypt.Net.BCrypt.HashPassword("a")
                }
            );
            context.SaveChanges();
        }
        else
        {
            Console.WriteLine("--> App already have User Data");
        }

    }
}