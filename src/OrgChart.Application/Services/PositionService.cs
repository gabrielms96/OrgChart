using OrgChart.Application.Common;
using OrgChart.Application.DTOs;
using OrgChart.Domain.Entities;
using OrgChart.Domain.Enums;
using OrgChart.Domain.Interfaces;

namespace OrgChart.Application.Services;

public interface IPositionService
{
    Task<Result<IEnumerable<PositionDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<PositionDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<PositionDto>> CreateAsync(PositionCreateDto dto, CancellationToken cancellationToken = default);
    Task<Result<PositionDto>> UpdateAsync(PositionUpdateDto dto, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public class PositionService : IPositionService
{
    private readonly IUnitOfWork _unitOfWork;

    public PositionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<PositionDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var positions = await _unitOfWork.Positions.GetAllAsync(cancellationToken);
        var dtos = positions.Select(MapToDto).ToList();
        return Result<IEnumerable<PositionDto>>.Success(dtos);
    }

    public async Task<Result<PositionDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var position = await _unitOfWork.Positions.GetByIdAsync(id, cancellationToken);
        if (position == null)
            return Result<PositionDto>.Failure("Cargo não encontrado");

        return Result<PositionDto>.Success(MapToDto(position));
    }

    public async Task<Result<PositionDto>> CreateAsync(PositionCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var position = new Position(dto.Name, dto.Level, dto.IsActive);
            await _unitOfWork.Positions.AddAsync(position, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<PositionDto>.Success(MapToDto(position));
        }
        catch (Exception ex)
        {
            return Result<PositionDto>.Failure($"Erro ao criar cargo: {ex.Message}");
        }
    }

    public async Task<Result<PositionDto>> UpdateAsync(PositionUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var position = await _unitOfWork.Positions.GetByIdAsync(dto.Id, cancellationToken);
            if (position == null)
                return Result<PositionDto>.Failure("Cargo não encontrado");

            position.Update(dto.Name, dto.Level, dto.IsActive);
            await _unitOfWork.Positions.UpdateAsync(position, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<PositionDto>.Success(MapToDto(position));
        }
        catch (Exception ex)
        {
            return Result<PositionDto>.Failure($"Erro ao atualizar cargo: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var position = await _unitOfWork.Positions.GetByIdAsync(id, cancellationToken);
            if (position == null)
                return Result.Failure("Cargo não encontrado");

            // Verificar se há colaboradores com este cargo
            if (position.Employees?.Any() == true)
                return Result.Failure("Não é possível excluir cargo com colaboradores vinculados");

            // Soft delete
            position.MarkAsDeleted();
            await _unitOfWork.Positions.UpdateAsync(position, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao excluir cargo: {ex.Message}");
        }
    }

    private PositionDto MapToDto(Position position)
    {
        return new PositionDto
        {
            Id = position.Id,
            Name = position.Name,
            Level = position.Level,
            LevelDescription = GetLevelDescription(position.Level),
            IsActive = position.IsActive,
            CreatedAt = position.CreatedAt,
            UpdatedAt = position.UpdatedAt,
            EmployeeCount = position.Employees?.Count ?? 0
        };
    }

    private string GetLevelDescription(EPositionLevel level)
    {
        return level switch
        {
            EPositionLevel.Intern => "Estagiário",
            EPositionLevel.Junior => "Júnior",
            EPositionLevel.MidLevel => "Pleno",
            EPositionLevel.Senior => "Sênior",
            EPositionLevel.Coordinator => "Coordenação",
            EPositionLevel.Manager => "Gerência",
            EPositionLevel.Director => "Diretoria",
            _ => level.ToString()
        };
    }
}
