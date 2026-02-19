using Microsoft.EntityFrameworkCore;
using OrgChart.Domain.Entities;
using OrgChart.Domain.Interfaces;
using OrgChart.Infrastructure.Data;

namespace OrgChart.Infrastructure.Repositories;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(OrgChartDbContext context) : base(context)
    {
    }

    public async Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(e => e.EmailAddress == email.ToLower(), cancellationToken);
    }

    public async Task<Employee?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Include(e => e.Manager)
            .Include(e => e.Subordinates)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Employee>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Include(e => e.Manager)
            .Include(e => e.Subordinates)
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Employee>> GetSubordinatesAsync(int managerId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Where(e => e.ManagerId == managerId)
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(e => e.Position)
            .Include(e => e.Manager)
            .Where(e => e.DepartmentId == departmentId)
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Employee>> GetRootEmployeesAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Include(e => e.Subordinates)
            .Where(e => e.ManagerId == null)
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);
    }
}
