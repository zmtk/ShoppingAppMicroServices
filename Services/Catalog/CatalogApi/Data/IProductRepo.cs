using CatalogApi.Models;

namespace CatalogApi.Data;

public interface IProductRepo
{
    bool SaveChanges();

    IEnumerable<Product> GetAllProducts();
    IEnumerable<Product>? GetFilteredProducts(string filter);
    Product? GetProductById(int id);
    IEnumerable<Product>? GetProductsByStoreId(int storeId);
    void CreateProduct(Product product);
    Product? UpdatePrice(int productId, double newPrice);
    bool DisableProductById(int id, bool storeDisable = false);
    bool DisableProductsByStore(int storeId);

}