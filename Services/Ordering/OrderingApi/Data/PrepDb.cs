using Microsoft.EntityFrameworkCore;

namespace OrderingApi.Data;
public static class PrepDb
{
    public static void MigrateDb(IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetService<AppDbContext>();

            if (context != null)
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
        }
    }

}
