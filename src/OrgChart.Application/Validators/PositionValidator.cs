using FluentValidation;
using OrgChart.Application.DTOs;

namespace OrgChart.Application.Validators;

public class PositionCreateDtoValidator : AbstractValidator<PositionCreateDto>
{
    public PositionCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome não pode ter mais de 200 caracteres");

        RuleFor(x => x.Level)
            .IsInEnum().WithMessage("Nível inválido");
    }
}

public class PositionUpdateDtoValidator : AbstractValidator<PositionUpdateDto>
{
    public PositionUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID inválido");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome não pode ter mais de 200 caracteres");

        RuleFor(x => x.Level)
            .IsInEnum().WithMessage("Nível inválido");
    }
}
