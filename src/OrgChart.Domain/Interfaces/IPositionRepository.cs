using OrgChart.Domain.Entities;

namespace OrgChart.Domain.Interfaces;

public interface IPositionRepository : IRepository<Position>
{
    Task<IEnumerable<Position>> GetActiveAsync(CancellationToken cancellationToken = default);
}
