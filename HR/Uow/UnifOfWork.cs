using HR.Data;
using HR.UoW;
public class UnitOfWork(
    AppDbContext context,
    IEmployeeRepository employeeRepo,
    ILeaveRepository leaveRepo) : IUnitOfWork
{
    private readonly AppDbContext _context = context;

    public IEmployeeRepository Employees { get; } = employeeRepo;
    public ILeaveRepository Leaves { get; } = leaveRepo;

    public async Task<int> SaveChangesAsync()
    {
         return await _context.SaveChangesAsync();
    }


    public void Dispose()
    {
        throw new NotImplementedException();
    }


}