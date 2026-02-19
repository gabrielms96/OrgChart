using OrgChart.Domain.Entities;

namespace OrgChart.Domain.Interfaces;

public interface IDepartmentRepository : IRepository<Department>
{
    Task<Department?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<Department>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Department>> GetActiveAsync(CancellationToken cancellationToken = default);
}
