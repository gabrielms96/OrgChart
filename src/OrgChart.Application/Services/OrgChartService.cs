using OrgChart.Application.Common;
using OrgChart.Application.DTOs;
using OrgChart.Domain.Entities;
using OrgChart.Domain.Interfaces;

namespace OrgChart.Application.Services;

public interface IOrgChartService
{
    Task<Result<IEnumerable<OrgChartNodeDto>>> GetOrgChartAsync(CancellationToken cancellationToken = default);
    Task<Result<OrgChartNodeDto>> GetOrgChartByEmployeeAsync(int employeeId, CancellationToken cancellationToken = default);
}

public class OrgChartService : IOrgChartService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrgChartService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<OrgChartNodeDto>>> GetOrgChartAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var rootEmployees = await _unitOfWork.Employees.GetRootEmployeesAsync(cancellationToken);
            var allEmployees = await _unitOfWork.Employees.GetAllWithDetailsAsync(cancellationToken);

            var employeesList = allEmployees.ToList();
            var nodes = new List<OrgChartNodeDto>();

            foreach (var root in rootEmployees)
            {
                var node = await BuildOrgChartNodeAsync(root, employeesList, cancellationToken);
                nodes.Add(node);
            }

            return Result<IEnumerable<OrgChartNodeDto>>.Success(nodes);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<OrgChartNodeDto>>.Failure($"Erro ao gerar organograma: {ex.Message}");
        }
    }

    public async Task<Result<OrgChartNodeDto>> GetOrgChartByEmployeeAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        try
        {
            var employee = await _unitOfWork.Employees.GetByIdWithDetailsAsync(employeeId, cancellationToken);
            if (employee == null)
                return Result<OrgChartNodeDto>.Failure("Colaborador não encontrado");

            var allEmployees = await _unitOfWork.Employees.GetAllWithDetailsAsync(cancellationToken);
            var node = await BuildOrgChartNodeAsync(employee, allEmployees.ToList(), cancellationToken);

            return Result<OrgChartNodeDto>.Success(node);
        }
        catch (Exception ex)
        {
            return Result<OrgChartNodeDto>.Failure($"Erro ao gerar organograma: {ex.Message}");
        }
    }

    private async Task<OrgChartNodeDto> BuildOrgChartNodeAsync(
        Employee employee,
        List<Employee> allEmployees,
        CancellationToken cancellationToken)
    {
        var node = new OrgChartNodeDto
        {
            Id = employee.Id,
            Name = employee.Name,
            Email = employee.EmailAddress,
            PositionName = employee.Position?.Name ?? "",
            PositionLevel = employee.Position?.Level.ToString() ?? "",
            DepartmentName = employee.Department?.Name ?? "",
            ManagerId = employee.ManagerId,
            Subordinates = new List<OrgChartNodeDto>()
        };

        var subordinates = allEmployees.Where(e => e.ManagerId == employee.Id).ToList();

        foreach (var subordinate in subordinates)
        {
            var subordinateNode = await BuildOrgChartNodeAsync(subordinate, allEmployees, cancellationToken);
            node.Subordinates.Add(subordinateNode);
        }

        return node;
    }
}
