using AutoMapper;
using CatalogApi.Data;
using CatalogApi.Dtos;
using CatalogApi.Models;
using Microsoft.AspNetCore.Mvc;
using Auth;
using Microsoft.IdentityModel.Tokens;


namespace CatalogApi.Controllers;


[Route("api/catalog/[controller]")]
[ApiController]
public class StoreController : ControllerBase
{
    private readonly IProductRepo _productRepo;
    private readonly IStoreRepo _storeRepo;
    private readonly IEmployeeRepo _employeeRepo;
    private readonly IMapper _mapper;

    public StoreController(
        IProductRepo productRepo,
        IStoreRepo storeRepo,
        IEmployeeRepo employeeRepo,
        IMapper mapper
    )
    {
        _productRepo = productRepo;
        _storeRepo = storeRepo;
        _employeeRepo = employeeRepo;
        _mapper = mapper;
    }

    [HttpGet("GetAllStores")]
    public ActionResult<IEnumerable<StoreReadDto>> GetAllStores()
    {
        var stores = _storeRepo.GetAllStores();

        return Ok(_mapper.Map<IEnumerable<StoreReadDto>>(stores));
    }


    [HttpGet]
    public ActionResult<List<Store>> GetStores()
    {
        if (TryGetUserIdFromToken(out int userId))
        {
            var Employee = _employeeRepo.GetEmployeesByUid(userId).ToList();
            List<Store> stores = new List<Store>();

            foreach (var emp in Employee)
            {
                stores.Add(_storeRepo.GetStoreById(emp.StoreId)!);
            }

            Console.WriteLine("yes");
            return Ok(stores);
        }

        return Ok();
    }

    private Employee? GetOperatingEmployee(Store store, string uid)
    {
        // Find the operating employee in the store
        return store.Employees.FirstOrDefault(e => e.UserId == int.Parse(uid));
    }

    private void CheckOperatingEmployeePermissions(Employee operatingEmployee, AddEmployeeDto addEmployeeDto)
    {
        // Check if there is no operating employee in the store
        if (operatingEmployee == null)
            throw new UnauthorizedAccessException("No employee in store");

        // Check if the operating employee is a manager or owner
        if (operatingEmployee.Group == 1000)
            throw new UnauthorizedAccessException("Only manager or owner can add employees");

        // Check if the new employee's group is valid based on the operating employee's group
        if (addEmployeeDto.Group == 1050 && operatingEmployee.Group != 1100)
            throw new UnauthorizedAccessException("Only Owners can add managers");

        if (addEmployeeDto.Group != 1000 && addEmployeeDto.Group != 1050)
            throw new UnauthorizedAccessException("Only Employee and Managers can be added");
    }

    private void AddOrUpdateEmployee(Store store, Employee? newEmployee, AddEmployeeDto addEmployeeDto)
    {
        if (newEmployee == null)
        {
            // If the new employee does not exist, add a new one
            store.Employees.Add(new Employee
            {
                UserId = addEmployeeDto.UserId,
                Group = addEmployeeDto.Group
            });
        }
        else
        {
            // If the new employee already exists, update its group (if not an owner)
            if (newEmployee.Group != 1100)
                newEmployee.Group = addEmployeeDto.Group;
        }
    }

    [HttpPost("addemployee")]
    public ActionResult<Employee> AddEmployee(AddEmployeeDto addEmployeeDto)
    {
        try
        {
            // Validate input parameters
            if (addEmployeeDto.UserId <= 0 || addEmployeeDto.StoreId <= 0)
                throw new ArgumentNullException();

            // Get store by Id
            Store? store = _storeRepo.GetStoreById(addEmployeeDto.StoreId);
            if (store == null)
                return NotFound("Store Not Found");

            // Get user Id from the access token
            if (TryGetUserIdFromToken(out int userId))
            {
                // Check if the operating employee is allowed to add employees
                Employee? operatingEmployee = GetOperatingEmployee(store, userId.ToString()!);
                CheckOperatingEmployeePermissions(operatingEmployee!, addEmployeeDto);

                // Check if the new employee already exists
                Employee? newEmployee = store.Employees.FirstOrDefault(e => e.UserId == addEmployeeDto.UserId);

                // Add or update the employee based on existence
                AddOrUpdateEmployee(store, newEmployee, addEmployeeDto);

                // Update the store in the repository
                _storeRepo.UpdateStore(store);

                return Ok();
            }

            throw new SecurityTokenExpiredException();
        }
        catch (SecurityTokenExpiredException)
        {
            return Unauthorized(new { message = "Access token is expired" });
            // Consider handling token expiration with token refresh logic
        }
        catch (ArgumentNullException)
        {
            return BadRequest(new { message = "UserId and StoreId must be provided" });
        }
    }

    [HttpPost]
    public ActionResult<StoreReadDto> AddStore(StoreCreateDto storeCreateDto)
    {
        try
        {
            var accessToken = Request.Headers.Authorization;
            bool accessTokenExist = !string.IsNullOrWhiteSpace(accessToken);

            // Get user Id from the access token
            if (TryGetUserIdFromToken(out int userId))
            {
                Console.WriteLine(userId);

                var storeModel = _mapper.Map<Store>(storeCreateDto);

                Employee employee = new Employee
                {
                    UserId = userId,
                    Group = 1100
                };

                storeModel.Employees.Add(employee);

                var store = _storeRepo.CreateStore(storeModel);
                _storeRepo.SaveChanges();
                var storeReadDto = _mapper.Map<StoreReadDto>(store);
                return Ok(storeReadDto);
            }

            throw new SecurityTokenExpiredException();
        }
        catch (SecurityTokenExpiredException)
        {
            return Unauthorized(new { message = "Access token is expired" });
        }
    }

    [HttpDelete("{storeId}")]
    public ActionResult<Employee> DisableStore(int storeId)
    {

        try
        {
            // Validate input parameters
            if (storeId <= 0)
                throw new ArgumentNullException();

            // Get store by Id
            Store? store = _storeRepo.GetStoreById(storeId);

            if (store == null)
                return NotFound("Store Not Found");

            // Get user Id from the access token
            if (TryGetUserIdFromToken(out int userId))
            {
                // Check if the operating employee is allowed to add employees
                Employee? operatingEmployee = GetOperatingEmployee(store, userId.ToString()!);
                if (operatingEmployee == null)
                    throw new UnauthorizedAccessException("Unauthorized!");

                if (operatingEmployee.Group != 1100)
                    throw new UnauthorizedAccessException("Only Owners can delete store");

                _productRepo.DisableProductsByStore(storeId);
                bool success = _storeRepo.DisableStore(storeId);
                if (success)
                {
                    return Ok(new { Message = "Store deleted successfully" });
                }
                else
                {
                    return NotFound(new { Message = "Something went wrong." });
                }

            }

            throw new SecurityTokenExpiredException();
        }
        catch (SecurityTokenExpiredException)
        {
            return Unauthorized(new { message = "Access token is expired" });
            // Consider handling token expiration with token refresh logic
        }
        catch (ArgumentNullException)
        {
            return BadRequest(new { message = "UserId and StoreId must be provided" });
        }
    }

    private bool TryGetUserIdFromToken(out int userId)
    {
        userId = 0; // Default value if parsing fails

        var accessToken = Request.Headers.Authorization;
        bool accessTokenExist = !string.IsNullOrWhiteSpace(accessToken);

        // Assuming GetUserId returns a string representation of the user ID
        if (accessTokenExist && int.TryParse(Authorize.GetUserId(accessToken), out userId))
        {
            return true;
        }

        return false;
    }


}