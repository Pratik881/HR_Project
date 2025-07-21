using HR.Models;

public interface IEmployeeRepository
{
     Task<List<Employee>> GetAllAsync();
    Task<Employee?> GetByIdAsync(int id);
    

    void UpdateEmployeeAsync(Employee employee);

    void  DeleteEmployeeAsync(Employee employee);

    Task<Employee?> GetByApplicationUserIdAsync(string applicationUserId);
   
    Task SaveChangesAsync();
}