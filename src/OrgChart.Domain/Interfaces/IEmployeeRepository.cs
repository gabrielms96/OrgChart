using OrgChart.Domain.Entities;

namespace OrgChart.Domain.Interfaces;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Employee?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Employee>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Employee>> GetSubordinatesAsync(int managerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Employee>> GetRootEmployeesAsync(CancellationToken cancellationToken = default);
}
