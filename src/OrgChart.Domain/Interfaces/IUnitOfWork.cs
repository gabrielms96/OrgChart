namespace OrgChart.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IDepartmentRepository Departments { get; }
    IPositionRepository Positions { get; }
    IEmployeeRepository Employees { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
