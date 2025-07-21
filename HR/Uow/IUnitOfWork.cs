namespace HR.UoW;
public interface IUnitOfWork : IDisposable
{
    IEmployeeRepository Employees { get; }
    ILeaveRepository Leaves { get; }

    Task<int> SaveChangesAsync();
}