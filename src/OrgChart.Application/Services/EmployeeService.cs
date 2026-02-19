using OrgChart.Application.Common;
using OrgChart.Application.DTOs;
using OrgChart.Domain.Entities;
using OrgChart.Domain.Interfaces;
using OrgChart.Domain.Services;
using OrgChart.Domain.ValueObjects;

namespace OrgChart.Application.Services;

public interface IEmployeeService
{
    Task<Result<IEnumerable<EmployeeDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<EmployeeDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<EmployeeDto>> CreateAsync(EmployeeCreateDto dto, CancellationToken cancellationToken = default);
    Task<Result<EmployeeDto>> UpdateAsync(EmployeeUpdateDto dto, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<EmployeeDto>>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default);
}

public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly HierarchyValidationService _hierarchyService;

    public EmployeeService(IUnitOfWork unitOfWork, HierarchyValidationService hierarchyService)
    {
        _unitOfWork = unitOfWork;
        _hierarchyService = hierarchyService;
    }

    public async Task<Result<IEnumerable<EmployeeDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var employees = await _unitOfWork.Employees.GetAllWithDetailsAsync(cancellationToken);
        var dtos = employees.Select(MapToDto).ToList();
        return Result<IEnumerable<EmployeeDto>>.Success(dtos);
    }

    public async Task<Result<EmployeeDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await _unitOfWork.Employees.GetByIdWithDetailsAsync(id, cancellationToken);
        if (employee == null)
            return Result<EmployeeDto>.Failure("Colaborador não encontrado");

        return Result<EmployeeDto>.Success(MapToDto(employee));
    }

    public async Task<Result<EmployeeDto>> CreateAsync(EmployeeCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingEmail = await _unitOfWork.Employees.GetByEmailAsync(dto.Email, cancellationToken);
            if (existingEmail != null)
                return Result<EmployeeDto>.Failure("Já existe um colaborador com este email");

            var department = await _unitOfWork.Departments.GetByIdAsync(dto.DepartmentId, cancellationToken);
            if (department == null)
                return Result<EmployeeDto>.Failure("Departamento não encontrado");

            var position = await _unitOfWork.Positions.GetByIdAsync(dto.PositionId, cancellationToken);
            if (position == null)
                return Result<EmployeeDto>.Failure("Cargo não encontrado");

            if (dto.ManagerId.HasValue)
            {
                var manager = await _unitOfWork.Employees.GetByIdAsync(dto.ManagerId.Value, cancellationToken);
                if (manager == null)
                    return Result<EmployeeDto>.Failure("Gerente não encontrado");
            }

            var email = Email.Create(dto.Email);
            var employee = new Employee(dto.Name, email, dto.DepartmentId, dto.PositionId, dto.HireDate, dto.ManagerId);

            await _unitOfWork.Employees.AddAsync(employee, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            employee = await _unitOfWork.Employees.GetByIdWithDetailsAsync(employee.Id, cancellationToken);
            return Result<EmployeeDto>.Success(MapToDto(employee!));
        }
        catch (ArgumentException ex)
        {
            return Result<EmployeeDto>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<EmployeeDto>.Failure($"Erro ao criar colaborador: {ex.Message}");
        }
    }

    public async Task<Result<EmployeeDto>> UpdateAsync(EmployeeUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var employee = await _unitOfWork.Employees.GetByIdWithDetailsAsync(dto.Id, cancellationToken);
            if (employee == null)
                return Result<EmployeeDto>.Failure("Colaborador não encontrado");

            var existingEmail = await _unitOfWork.Employees.GetByEmailAsync(dto.Email, cancellationToken);
            if (existingEmail != null && existingEmail.Id != dto.Id)
                return Result<EmployeeDto>.Failure("Já existe um colaborador com este email");

            var department = await _unitOfWork.Departments.GetByIdAsync(dto.DepartmentId, cancellationToken);
            if (department == null)
                return Result<EmployeeDto>.Failure("Departamento não encontrado");

            var position = await _unitOfWork.Positions.GetByIdAsync(dto.PositionId, cancellationToken);
            if (position == null)
                return Result<EmployeeDto>.Failure("Cargo não encontrado");

            if (dto.ManagerId.HasValue)
            {
                var manager = await _unitOfWork.Employees.GetByIdAsync(dto.ManagerId.Value, cancellationToken);
                if (manager == null)
                    return Result<EmployeeDto>.Failure("Gerente não encontrado");

                var isValid = await _hierarchyService.ValidateHierarchyAsync(dto.Id, dto.ManagerId, cancellationToken);
                if (!isValid)
                    return Result<EmployeeDto>.Failure("Esta atribuição criaria um ciclo na hierarquia organizacional");
            }

            var email = Email.Create(dto.Email);
            employee.Update(dto.Name, email, dto.DepartmentId, dto.PositionId, dto.HireDate);
            employee.ChangeManager(dto.ManagerId);

            await _unitOfWork.Employees.UpdateAsync(employee, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            employee = await _unitOfWork.Employees.GetByIdWithDetailsAsync(employee.Id, cancellationToken);
            return Result<EmployeeDto>.Success(MapToDto(employee!));
        }
        catch (ArgumentException ex)
        {
            return Result<EmployeeDto>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<EmployeeDto>.Failure($"Erro ao atualizar colaborador: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id, cancellationToken);
            if (employee == null)
                return Result.Failure("Colaborador não encontrado");

            var subordinates = await _unitOfWork.Employees.GetSubordinatesAsync(id, cancellationToken);
            if (subordinates.Any())
                return Result.Failure("Não é possível excluir colaborador que possui subordinados");

            employee.MarkAsDeleted();
            await _unitOfWork.Employees.UpdateAsync(employee, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao excluir colaborador: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<EmployeeDto>>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default)
    {
        var employees = await _unitOfWork.Employees.GetByDepartmentAsync(departmentId, cancellationToken);
        var dtos = employees.Select(MapToDto).ToList();
        return Result<IEnumerable<EmployeeDto>>.Success(dtos);
    }

    private EmployeeDto MapToDto(Employee employee)
    {
        return new EmployeeDto
        {
            Id = employee.Id,
            Name = employee.Name,
            Email = employee.EmailAddress,
            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.Department?.Name ?? "",
            PositionId = employee.PositionId,
            PositionName = employee.Position?.Name ?? "",
            PositionLevel = employee.Position?.Level.ToString() ?? "",
            HireDate = employee.HireDate,
            ManagerId = employee.ManagerId,
            ManagerName = employee.Manager?.Name,
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt,
            SubordinatesCount = employee.Subordinates?.Count ?? 0
        };
    }
}
