using CatalogApi.Models;

namespace CatalogApi.Data;

public class EmployeeRepo : IEmployeeRepo
{
    private readonly AppDbContext _context;

    public EmployeeRepo(AppDbContext context)
    {
        _context = context;
    }

    public Employee CreateEmployee(Employee employee)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Employee> GetAllEmployees()
    {
        throw new NotImplementedException();
    }

    public Employee? GetEmployeeById(int employeeId)
    {
        return _context.Employees.FirstOrDefault(e => e.Id == employeeId);
    }

    public IEnumerable<Employee> GetEmployeesByUid(int userId)
    {
        return _context.Employees.Where(e => e.UserId == userId);
    }

    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }
}