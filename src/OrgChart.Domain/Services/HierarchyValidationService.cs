using OrgChart.Domain.Entities;
using OrgChart.Domain.Interfaces;

namespace OrgChart.Domain.Services;

public class HierarchyValidationService
{
    private readonly IEmployeeRepository _employeeRepository;

    public HierarchyValidationService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<bool> ValidateHierarchyAsync(int employeeId, int? managerId, CancellationToken cancellationToken = default)
    {

        if (!managerId.HasValue)
            return true;

        if (employeeId == managerId.Value)
            return false;

        return !await IsSubordinateAsync(employeeId, managerId.Value, cancellationToken);
    }

    private async Task<bool> IsSubordinateAsync(int managerId, int potentialSubordinateId, CancellationToken cancellationToken)
    {
        var visited = new HashSet<int>();
        var queue = new Queue<int>();

        var subordinates = await _employeeRepository.GetSubordinatesAsync(managerId, cancellationToken);

        foreach (var subordinate in subordinates)
        {
            queue.Enqueue(subordinate.Id);
        }

        while (queue.Count > 0)
        {
            var currentId = queue.Dequeue();

            if (!visited.Add(currentId))
                continue;

            if (currentId == potentialSubordinateId)
                return true;

            var currentSubordinates = await _employeeRepository.GetSubordinatesAsync(currentId, cancellationToken);
            foreach (var sub in currentSubordinates)
            {
                if (!visited.Contains(sub.Id))
                    queue.Enqueue(sub.Id);
            }
        }

        return false;
    }

    public async Task<IEnumerable<Employee>> GetHierarchyChainAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        var chain = new List<Employee>();
        var currentId = employeeId;

        while (currentId != 0)
        {
            var employee = await _employeeRepository.GetByIdWithDetailsAsync(currentId, cancellationToken);
            if (employee == null)
                break;

            chain.Add(employee);
            currentId = employee.ManagerId ?? 0;
        }

        return chain;
    }

    public async Task<IEnumerable<Employee>> GetSubordinatesTreeAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        var tree = new List<Employee>();
        await BuildSubordinatesTreeAsync(employeeId, tree, cancellationToken);
        return tree;
    }

    private async Task BuildSubordinatesTreeAsync(int managerId, List<Employee> tree, CancellationToken cancellationToken)
    {
        var subordinates = await _employeeRepository.GetSubordinatesAsync(managerId, cancellationToken);

        foreach (var subordinate in subordinates)
        {
            tree.Add(subordinate);
            await BuildSubordinatesTreeAsync(subordinate.Id, tree, cancellationToken);
        }
    }
}
