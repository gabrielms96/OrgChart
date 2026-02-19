using FluentValidation;
using OrgChart.Application.DTOs;

namespace OrgChart.Application.Validators;

public class DepartmentCreateDtoValidator : AbstractValidator<DepartmentCreateDto>
{
    public DepartmentCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome não pode ter mais de 200 caracteres");

        RuleFor(x => x.Code)
            .MaximumLength(50).WithMessage("Código não pode ter mais de 50 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Code));
    }
}

public class DepartmentUpdateDtoValidator : AbstractValidator<DepartmentUpdateDto>
{
    public DepartmentUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID inválido");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome não pode ter mais de 200 caracteres");

        RuleFor(x => x.Code)
            .MaximumLength(50).WithMessage("Código não pode ter mais de 50 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Code));
    }
}
