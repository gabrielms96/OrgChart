using Microsoft.EntityFrameworkCore;
using OrgChart.Domain.Entities;
using OrgChart.Domain.Interfaces;
using OrgChart.Infrastructure.Data;

namespace OrgChart.Infrastructure.Repositories;

public class DepartmentRepository : Repository<Department>, IDepartmentRepository
{
    public DepartmentRepository(OrgChartDbContext context) : base(context)
    {
    }

    public async Task<Department?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(d => d.Code == code, cancellationToken);
    }

    public async Task<IEnumerable<Department>> SearchByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(d => d.Name.Contains(name))
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Department>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    public override async Task<IEnumerable<Department>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(d => d.Employees)
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    public override async Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(d => d.Employees)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }
}
