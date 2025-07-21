using HR.DTO;
using HR.Models;
using HR.Utilities;
using System.Threading.Tasks;

namespace HR.Interfaces
{
    public interface IEmployeeService
    {
        Task<ServiceResponse<List<Employee>>> GetAllEmployees();

        Task<ServiceResponse<Employee>> GetEmployeeById(int id);
        Task<ServiceResponse<Employee>> GetEmployeeByUserId(string userId);

       Task<ServiceResponse<string>> UpdateEmployee(Employee employee);

        Task<ServiceResponse<string>> DeleteEmployeeById(int id);


    }
}
