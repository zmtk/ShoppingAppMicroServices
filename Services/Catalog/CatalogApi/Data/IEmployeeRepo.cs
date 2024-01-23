using CatalogApi.Models;

namespace CatalogApi.Data;

public interface IEmployeeRepo
{
    bool SaveChanges();
    IEnumerable<Employee> GetAllEmployees();
    IEnumerable<Employee> GetEmployeesByUid(int userId);
    Employee? GetEmployeeById(int employeeId);
    Employee CreateEmployee(Employee employee);
}