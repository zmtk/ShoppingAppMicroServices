using Microsoft.EntityFrameworkCore;
using CatalogApi.Models;

namespace CatalogApi.Data;

public class StoreRepo : IStoreRepo
{
    private readonly AppDbContext _context;

    public StoreRepo(AppDbContext context)
    {
        _context = context;
    }

    public Store CreateStore(Store store)
    {
        if (store == null)
            throw new ArgumentNullException(nameof(store));

        _context.Stores.Add(store);

        return store;
    }

    public IEnumerable<Store> GetAllStores()
    {
        return _context.Stores
                    .Include(o => o.Employees)
                    .ToList();
    }
    public Store UpdateStore(Store store)
    {
        // Assuming Store has an 'Id' property that uniquely identifies the store
        var existingStore = _context.Stores.Find(store.Id);

        if (existingStore != null)
        {
            // Update the properties of the existing store with the new values
            existingStore = store;
            // Update other properties as needed

            _context.SaveChanges(); // Save changes to the database
        }
        // Handle the case where the store is not found if needed


        return store;
    }

    public Store? GetStoreById(int storeId)
    {
        return _context.Stores
                    .Include(o => o.Employees)
                    .FirstOrDefault(s => s.Id == storeId);
    }

    public bool DisableStore(int storeId)
    {
        var storeToDisable = _context.Stores.FirstOrDefault(p => p.Id == storeId);

        if (storeToDisable != null)
        {
            storeToDisable.Inactive=true;
            
            // _context.Stores.Remove(storeToDelete);
            _context.SaveChanges();
            return true; // Store successfully deleted
        }

        return false; // Store with the specified ID was not found
    }

    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }
}
