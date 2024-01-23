using AutoMapper;
using CatalogApi.AsyncDataServices;
using CatalogApi.Data;
using CatalogApi.Dtos;
using CatalogApi.Models;
using Microsoft.AspNetCore.Mvc;
using Auth;
using Grpc.Net.Client;
using BasketApi;

namespace CatalogApi.Controllers;

[Route("api/catalog/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductRepo _productRepo;
    private readonly IStoreRepo _storeRepo;
    private readonly IMapper _mapper;
    private readonly IMessageBusClient _messageBusClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductRepo productRepo,
        IStoreRepo storeRepo,
        IMapper mapper,
        IMessageBusClient messageBusClient,
        IConfiguration configuration,
        ILogger<ProductsController> logger
    )
    {
        _productRepo = productRepo;
        _storeRepo = storeRepo;
        _mapper = mapper;
        _messageBusClient = messageBusClient;
        _configuration = configuration;
        _logger = logger;
    }
    [HttpGet]
    public ActionResult<IEnumerable<ProductReadDto>> GetAllProducts()
    {
        Console.WriteLine("--> Getting Products...");
        var products = _productRepo.GetAllProducts();

        return Ok(_mapper.Map<IEnumerable<ProductReadDto>>(products));
    }

    [HttpGet("{filter}")]
    public ActionResult<IEnumerable<ProductReadDto>> GetFilteredProducts(string filter)
    {
        Console.WriteLine($"--> Getting Filtered Products...{filter}");

        var products = _productRepo.GetFilteredProducts(filter);

        return Ok(_mapper.Map<IEnumerable<ProductReadDto>>(products));
    }

    [HttpGet("id/{id}", Name = "GetProductById")]
    public ActionResult<ProductReadDto> GetProductById(int id)
    {
        var product = _productRepo.GetProductById(id);
        if (product != null)
        {
            return Ok(_mapper.Map<ProductReadDto>(product));
        }

        return NotFound();
    }

    [HttpGet("store/{storeId}", Name = "GetProductsByStore")]
    public ActionResult<ProductReadDto> GetProductByStore(int storeId)
    {
        var products = _productRepo.GetProductsByStoreId(storeId);
        if (products != null)
        {
            return Ok(_mapper.Map<IEnumerable<ProductReadDto>>(products));
        }

        return NotFound();
    }

    private bool TryGetUserIdFromToken(out int userId)
    {
        userId = -1; // Default value if parsing fails

        var accessToken = Request.Headers.Authorization;
        bool accessTokenExist = !string.IsNullOrWhiteSpace(accessToken);

        // Assuming GetUserId returns a string representation of the user ID
        if (accessTokenExist && int.TryParse(Authorize.GetUserId(accessToken), out userId))
        {
            return true;
        }

        return false;
    }

    [HttpPost("updateprice")]
    public ActionResult<ProductReadDto> UpdatePrice(UpdatePriceDto updatePriceDto)
    {
        try
        {
            if (TryGetUserIdFromToken(out int userId))
            {
                // Retrieve the product by its ID
                Product? product = _productRepo.GetProductById(updatePriceDto.Id);

                if (product == null)
                {
                    // 404 Not Found
                    return NotFound(new { ErrorMessage = "Product Not Found" });
                }

                // Retrieve the store by the product's StoreId
                Store? store = _storeRepo.GetStoreById(product.StoreId);

                if (store == null)
                {
                    // 404 Not Found
                    return NotFound(new { ErrorMessage = "Store Not Found" });
                }

                Employee? employee = store.Employees.FirstOrDefault(e => e.UserId == userId);

                if (employee == null)
                {
                    // 401 Unauthorized
                    return Unauthorized(new { ErrorMessage = "Unauthorized" });
                }

                // Check if the new price is different from the existing price
                if (updatePriceDto.NewPrice != product.Price)
                {
                    // Update the price
                    var updatedProduct = _productRepo.UpdatePrice(updatePriceDto.Id, updatePriceDto.NewPrice);

                    if (updatedProduct != null)
                    {
                        var productReadDto = _mapper.Map<ProductReadDto>(updatedProduct);

                        // Send Updated Price message to basketapi
                        var updateBasketPriceDto = _mapper.Map<UpdateBasketPriceDto>(productReadDto);
                        Console.WriteLine("ProductID:" + updateBasketPriceDto.Id);
                        Console.WriteLine("NewPrice:" + updateBasketPriceDto.NewPrice);
                        Console.WriteLine("Event:" + updateBasketPriceDto.Event);
                        _messageBusClient.UpdateBasketPrices(updateBasketPriceDto);

                        return Ok(productReadDto);
                    }
                }

                // The new price is the same as the existing price, no update needed
                return Ok(new { Message = $"Price is already {updatePriceDto.NewPrice}" });
            }
            else
            {
                // 401 Unauthorized
                return Unauthorized(new { ErrorMessage = "Unauthorized" });
            }
        }
        catch (Exception ex)
        {
            // Log the exception
            _logger.LogError(ex, "Error updating product price");

            // 500 Internal Server Error
            return StatusCode(500, new { ErrorMessage = "Internal Server Error" });
        }
    }

    [HttpPut("{productId}")]
    public ActionResult<ProductReadDto> DeleteProduct(int productId)
    {
        try
        {
            if (TryGetUserIdFromToken(out int userId))
            {
                // Retrieve the product by its ID
                Product? product = _productRepo.GetProductById(productId);

                if (product == null)
                {
                    // 404 Not Found
                    return NotFound(new { ErrorMessage = "Product Not Found" });
                }

                // Retrieve the store by the product's StoreId
                Store? store = _storeRepo.GetStoreById(product.StoreId);

                if (store == null)
                {
                    // 404 Not Found
                    return NotFound(new { ErrorMessage = "Store Not Found" });
                }

                Employee? employee = store.Employees.FirstOrDefault(e => e.UserId == userId);

                if (employee == null)
                {
                    // 401 Unauthorized
                    return Unauthorized(new { ErrorMessage = "Unauthorized" });
                }

                var isDisabled = _productRepo.DisableProductById(productId);

                if (isDisabled)
                {
                    return Ok(new { Message = $"Product {(product.Inactive ? "disabled":"enabled" )} successfully" } );
                }
                else
                {
                    return NotFound(new { ErrorMessage = "Product not found" } );
                }

            }
            else
            {
                // 401 Unauthorized
                return Unauthorized(new { ErrorMessage = "Unauthorized" });
            }
        }
        catch (Exception ex)
        {
            // Log the exception
            _logger.LogError(ex, "Error updating product price");

            // 500 Internal Server Error
            return StatusCode(500, new { ErrorMessage = "Internal Server Error" });
        }
    }

    [HttpPost]
    public ActionResult<ProductReadDto> CreateProduct(ProductCreateDto productCreateDto)
    {

        if (TryGetUserIdFromToken(out int userId))
        {
            Store? store = _storeRepo.GetStoreById(productCreateDto.StoreId);

            if (store == null)
            {
                // 404 Not Found
                return NotFound(new { ErrorMessage = "Store Not Found" });
            }

            Employee? employee = store.Employees.FirstOrDefault(e => e.UserId == userId);

            if (employee == null)
            {
                // 401 Unauthorized
                return Unauthorized(new { ErrorMessage = "Unauthorized" });
            }

            try
            {
                var productModel = _mapper.Map<Product>(productCreateDto);

                // Begin a transaction (if applicable)
                // _productRepo.BeginTransaction();

                _productRepo.CreateProduct(productModel);
                _productRepo.SaveChanges();

                // Commit the transaction (if applicable)
                // _productRepo.CommitTransaction();

                var productReadDto = _mapper.Map<ProductReadDto>(productModel);

                // 201 Created
                return CreatedAtRoute(nameof(GetProductById), new { Id = productReadDto.Id }, productReadDto);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error creating a product");

                // Rollback the transaction (if applicable)
                // _productRepo.RollbackTransaction();

                // 500 Internal Server Error
                return StatusCode(500, new { ErrorMessage = "Internal Server Error" });
            }
        }
        else
        {
            // 401 Unauthorized
            return Unauthorized(new { ErrorMessage = "Unauthorized" });
        }
    }


    [HttpPost("updatebasket")]
    public ActionResult UpdateBasket(CatchBasketDto catchBasket)
    {
        int productid = catchBasket.ProductId;
        var product = _productRepo.GetProductById(productid);

        if (product == null)
            return NotFound("Product not found");

        if (TryGetUserIdFromToken(out int userId))
        {

            var channel = GrpcChannel.ForAddress(_configuration["GrpcBasketApi"]!);
            var client = new BasketGrpc.BasketGrpcClient(channel);
            var request = new AddToBasketRequest
            {
                UserId = userId,
                ProductId = product.Id,
                Name = product.Name,
                Price = product.Price
            };

            try
            {
                var reply = client.AddToBasket(request);
                return Ok(new { reply.UserId, reply.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calling GRPC Server. Server Address: {_configuration["GrpcBasketApi"]}");
                return StatusCode(503, $"Service Unavailable: Unable to communicate with GRPC server. Details: {ex.Message}");
            }

        }
        else
        {
            return Unauthorized("Unauthorized");
        }


    }

}