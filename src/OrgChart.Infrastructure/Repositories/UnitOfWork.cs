using Microsoft.EntityFrameworkCore.Storage;
using OrgChart.Domain.Interfaces;
using OrgChart.Infrastructure.Data;

namespace OrgChart.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly OrgChartDbContext _context;
    private IDbContextTransaction? _transaction;

    public IDepartmentRepository Departments { get; }
    public IPositionRepository Positions { get; }
    public IEmployeeRepository Employees { get; }

    public UnitOfWork(
        OrgChartDbContext context,
        IDepartmentRepository departmentRepository,
        IPositionRepository positionRepository,
        IEmployeeRepository employeeRepository)
    {
        _context = context;
        Departments = departmentRepository;
        Positions = positionRepository;
        Employees = employeeRepository;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
