using CatalogApi.AsyncDataServices;
using CatalogApi.Dtos;
using CatalogApi.Models;

namespace CatalogApi.Data;

public class ProductRepo : IProductRepo
{
    private readonly AppDbContext _context;
    private readonly IMessageBusClient _messageBusClient;

    public ProductRepo(AppDbContext context, IMessageBusClient messageBusClient)
    {
        _context = context;
        _messageBusClient = messageBusClient;
    }

    public Product? UpdatePrice(int productId, double newPrice)
    {
        var product = _context.Products.Find(productId);

        if (product != null)
        {
            // Move the current price to OldPrice
            product.OldPrice = product.Price;

            // Update the price with the new value
            product.Price = newPrice;

            _context.SaveChanges();
        }

        return product;
    }


    public void CreateProduct(Product product)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));

        _context.Products.Add(product);
    }

    public IEnumerable<Product> GetAllProducts()
    {
        return _context.Products.ToList();
    }

    public IEnumerable<Product>? GetFilteredProducts(string filter)
    {
        return _context.Products
            .AsEnumerable()
            .Where(p => string.Equals(p.Gender, filter, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
    public bool DisableProductsByStore(int storeId)
    {
        var productsToDisable = _context.Products.Where(p => p.StoreId == storeId).ToList();

        if (productsToDisable.Any())
        {
            foreach (var product in productsToDisable)
            {
                DisableProductById(id:product.Id,storeDisable: true);
            }
            _context.SaveChanges();
            return true; // Products successfully deleted
        }

        return false; // No products found with the specified condition
    }

    public bool DisableProductById(int id, bool storeDisable = false)
    {
        var productToDisable = _context.Products.FirstOrDefault(p => p.Id == id);

        if (productToDisable != null)
        {
            productToDisable.Inactive = storeDisable ? storeDisable : !productToDisable.Inactive;

            var toggleProductFromBasketDto = new ToggleProductFromBasketDto
            {
                Id = productToDisable.Id,
                Event = productToDisable.Inactive ? "Event_Disable_Product_From_Basket" : "Event_Enable_Product_From_Basket"
            };

            _messageBusClient.ToggleProductFromBasket(toggleProductFromBasketDto);
            _context.SaveChanges();
            return true; // Product successfully deleted
        }

        return false; // Product with the specified ID was not found
    }

    public Product? GetProductById(int id)
    {
        return _context.Products.FirstOrDefault(p => p.Id == id);
    }

    public IEnumerable<Product>? GetProductsByStoreId(int storeId)
    {
        return _context.Products
            .AsEnumerable()
            .Where(p => p.StoreId == storeId)
            .ToList();
    }

    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }
}