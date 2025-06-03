using HR.Data;
using HR.DTO;
using HR.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HR.Utilities;
using HR.Interfaces;

namespace HR.Services
{
    public class EmployeeService:IEmployeeService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeeService(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Get all employees
        public async Task<ServiceResponse<List<Employee>>> GetAllEmployees()
        {
            var employees = await _context.Employees.ToListAsync();
            return ServiceResponse<List<Employee>>.Ok(employees); // Return all employees
        }

        // Get employee by ID
        public async Task<ServiceResponse<Employee>> GetEmployeeById(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return ServiceResponse<Employee>.Fail("Employee not found.");
            }
            return ServiceResponse<Employee>.Ok(employee); // Return employee if found
        }

        // Update employee
        public async Task<ServiceResponse<string>> UpdateEmployee(Employee employee)
        {
            var existingEmployee = await _context.Employees.FindAsync(employee.Id);
            if (existingEmployee == null)
            {
                return ServiceResponse<string>.Fail("Employee not found.");
            }

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
            return ServiceResponse<string>.Ok("Employee updated successfully.");
        }

        // Delete employee by ID
        public async Task<ServiceResponse<string>> DeleteEmployeeById(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return ServiceResponse<string>.Fail("Employee not found.");
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return ServiceResponse<string>.Ok("Employee deleted successfully.");
        }
    }
}
