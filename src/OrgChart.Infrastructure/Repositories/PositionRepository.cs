using Microsoft.EntityFrameworkCore;
using OrgChart.Domain.Entities;
using OrgChart.Domain.Interfaces;
using OrgChart.Infrastructure.Data;

namespace OrgChart.Infrastructure.Repositories;

public class PositionRepository : Repository<Position>, IPositionRepository
{
    public PositionRepository(OrgChartDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Position>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.IsActive)
            .OrderBy(p => p.Level)
            .ThenBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public override async Task<IEnumerable<Position>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Employees)
            .OrderBy(p => p.Level)
            .ThenBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public override async Task<Position?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Employees)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
}
