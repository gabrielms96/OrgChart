using OrgChart.Application.Common;
using OrgChart.Application.DTOs;
using OrgChart.Domain.Entities;
using OrgChart.Domain.Interfaces;

namespace OrgChart.Application.Services;

public interface IDepartmentService
{
    Task<Result<IEnumerable<DepartmentDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<DepartmentDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<DepartmentDto>>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Result<DepartmentDto>> CreateAsync(DepartmentCreateDto dto, CancellationToken cancellationToken = default);
    Task<Result<DepartmentDto>> UpdateAsync(DepartmentUpdateDto dto, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public class DepartmentService : IDepartmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public DepartmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<DepartmentDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var departments = await _unitOfWork.Departments.GetAllAsync(cancellationToken);
        var dtos = departments.Select(MapToDto).ToList();
        return Result<IEnumerable<DepartmentDto>>.Success(dtos);
    }

    public async Task<Result<DepartmentDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(id, cancellationToken);
        if (department == null)
            return Result<DepartmentDto>.Failure("Departamento não encontrado");

        return Result<DepartmentDto>.Success(MapToDto(department));
    }

    public async Task<Result<IEnumerable<DepartmentDto>>> SearchByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var departments = await _unitOfWork.Departments.SearchByNameAsync(name, cancellationToken);
        var dtos = departments.Select(MapToDto).ToList();
        return Result<IEnumerable<DepartmentDto>>.Success(dtos);
    }

    public async Task<Result<DepartmentDto>> CreateAsync(DepartmentCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(dto.Code))
            {
                var existing = await _unitOfWork.Departments.GetByCodeAsync(dto.Code, cancellationToken);
                if (existing != null)
                    return Result<DepartmentDto>.Failure("Já existe um departamento com este código");
            }

            var department = new Department(dto.Name, dto.Code, dto.IsActive);
            await _unitOfWork.Departments.AddAsync(department, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<DepartmentDto>.Success(MapToDto(department));
        }
        catch (Exception ex)
        {
            return Result<DepartmentDto>.Failure($"Erro ao criar departamento: {ex.Message}");
        }
    }

    public async Task<Result<DepartmentDto>> UpdateAsync(DepartmentUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(dto.Id, cancellationToken);
            if (department == null)
                return Result<DepartmentDto>.Failure("Departamento não encontrado");

            if (!string.IsNullOrWhiteSpace(dto.Code))
            {
                var existing = await _unitOfWork.Departments.GetByCodeAsync(dto.Code, cancellationToken);
                if (existing != null && existing.Id != dto.Id)
                    return Result<DepartmentDto>.Failure("Já existe um departamento com este código");
            }

            department.Update(dto.Name, dto.Code, dto.IsActive);
            await _unitOfWork.Departments.UpdateAsync(department, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<DepartmentDto>.Success(MapToDto(department));
        }
        catch (Exception ex)
        {
            return Result<DepartmentDto>.Failure($"Erro ao atualizar departamento: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id, cancellationToken);
            if (department == null)
                return Result.Failure("Departamento não encontrado");

            var employees = await _unitOfWork.Employees.GetByDepartmentAsync(id, cancellationToken);
            if (employees.Any())
                return Result.Failure("Não é possível excluir departamento com colaboradores vinculados");

            department.MarkAsDeleted();
            await _unitOfWork.Departments.UpdateAsync(department, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao excluir departamento: {ex.Message}");
        }
    }

    private DepartmentDto MapToDto(Department department)
    {
        return new DepartmentDto
        {
            Id = department.Id,
            Name = department.Name,
            Code = department.Code,
            IsActive = department.IsActive,
            CreatedAt = department.CreatedAt,
            UpdatedAt = department.UpdatedAt,
            EmployeeCount = department.Employees?.Count ?? 0
        };
    }
}
