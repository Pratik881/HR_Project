using HR.Data;
using HR.Models;
using Microsoft.EntityFrameworkCore;

namespace HR.Repository;

public class LeaveRepository(AppDbContext context) : ILeaveRepository
{
    private readonly AppDbContext _context = context;
    public  async Task AddAsync(Leave leave)
    {
        await _context.Leaves.AddAsync(leave);
    }

    public async Task<Leave?> GetByIdAsync(int id)
    {
        return await _context.Leaves.FindAsync(id);
    }

    public async  Task<List<Leave>> GetLeavesByEmployeeIdAsync(int employeeId)
    {
        var leaves = await _context.Leaves.Where(l => l.ApplicationUserId == employeeId).ToListAsync();
        return leaves;
    }

    public async Task<List<Leave>> GetLeavesByStatusAsync(LeaveStatus status)
    {
        var leaves = await _context.Leaves.Where(l => l.Status == status).ToListAsync();
        return leaves;
    }

    public async Task<List<Leave>> GetApprovedLeavesForDateAsync(DateTime date, LeaveStatus status)
    {
        var leaves = await _context.Leaves.Where(l => l.Status == LeaveStatus.Pending &&
        l.StartDate <= date && l.EndDate >= date).ToListAsync();

        return leaves;
    }

     public  async Task  SaveChangesAsync()
    {
       await  _context.SaveChangesAsync();
    }
}