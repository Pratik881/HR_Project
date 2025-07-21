using HR.Data;
using HR.DTO;
using HR.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HR.Utilities;
using HR.Interfaces;
public class EmployeeRepository(AppDbContext context) : IEmployeeRepository
{
    private readonly AppDbContext _context = context;

     public void  DeleteEmployeeAsync(Employee employee)
    {
        _context.Employees.Remove(employee);
    }


    public  Task<List<Employee>> GetAllAsync()
    {
        return  _context.Employees.ToListAsync();
    }

    public async Task<Employee?> GetByApplicationUserIdAsync(string applicationUserId)
    {
         return await _context.Employees.FirstOrDefaultAsync(e=>e.ApplicationUserId==applicationUserId);
    }

    public async Task<Employee?> GetByIdAsync(int  Id)
    {
        return await _context.Employees.FindAsync(Id);
    }


    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void  UpdateEmployeeAsync(Employee employee)
    {
        _context.Employees.Update(employee);
    }

  
}