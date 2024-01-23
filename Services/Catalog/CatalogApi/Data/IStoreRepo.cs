using CatalogApi.Models;

namespace CatalogApi.Data;

public interface IStoreRepo
{
    bool SaveChanges();
    IEnumerable<Store> GetAllStores();
    
    // IEnumerable<Store> GetStores(int emlpoyee);
    Store UpdateStore(Store store);
    Store? GetStoreById(int storeId);
    Store CreateStore(Store store);
    bool DisableStore(int storeId);

}