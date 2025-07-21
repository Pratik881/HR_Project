using HR.Models;

public interface ILeaveRepository
{
    Task AddAsync(Leave leave);
    Task<Leave?> GetByIdAsync(int id);

    Task<List<Leave>> GetLeavesByEmployeeIdAsync(int employeeId);
    Task<List<Leave>> GetLeavesByStatusAsync(LeaveStatus status);

    Task<List<Leave>> GetApprovedLeavesForDateAsync(DateTime date, LeaveStatus status);
    Task  SaveChangesAsync();
}