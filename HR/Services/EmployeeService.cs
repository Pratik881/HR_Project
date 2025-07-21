using HR.Models;
using HR.Utilities;
using HR.Interfaces;
using HR.UoW;

namespace HR.Services
{
    public class EmployeeService(IUnitOfWork unitOfWork) : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        

        public async Task<ServiceResponse<List<Employee>>> GetAllEmployees()
        {
            var employees = await  _unitOfWork.Employees.GetAllAsync();
            return ServiceResponse<List<Employee>>.Ok(employees); // Return all employees
        }

        // Get employee by ID
        public async Task<ServiceResponse<Employee>> GetEmployeeByUserId(string userId)
        {
            var employee =await  _unitOfWork.Employees.GetByApplicationUserIdAsync(userId);
            if (employee == null)
            {
                return ServiceResponse<Employee>.Fail("Employee not found.");
            }
            return ServiceResponse<Employee>.Ok(employee); // Return employee if found
        }

        // Update employee
        public async Task<ServiceResponse<string>> UpdateEmployee(Employee employee)
        {
            var existingEmployee = await _unitOfWork.Employees.GetByIdAsync(employee.Id);

            if (existingEmployee == null)
            {
                return ServiceResponse<string>.Fail("Employee not found.");
            }

           _unitOfWork.Employees.UpdateEmployeeAsync(employee);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResponse<string>.Ok("Employee updated successfully.");
        }

        // Delete employee by ID
        public async Task<ServiceResponse<string>> DeleteEmployeeById(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
            {
                return ServiceResponse<string>.Fail("Employee not found.");
            }

            _unitOfWork.Employees.DeleteEmployeeAsync(employee);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResponse<string>.Ok("Employee deleted successfully.");
        }

        public async Task<ServiceResponse<Employee>> GetEmployeeById(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
            {
                return ServiceResponse<Employee>.Fail("Employee not found.");
            }
            else
            {
                 return ServiceResponse<Employee>.Ok(employee);
            }
        }

    }
}
