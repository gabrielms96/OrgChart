using FluentValidation;
using OrgChart.Application.DTOs;

namespace OrgChart.Application.Validators;

public class EmployeeCreateDtoValidator : AbstractValidator<EmployeeCreateDto>
{
    public EmployeeCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome não pode ter mais de 200 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido")
            .MaximumLength(255).WithMessage("Email não pode ter mais de 255 caracteres");

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("Departamento é obrigatório");

        RuleFor(x => x.PositionId)
            .GreaterThan(0).WithMessage("Cargo é obrigatório");

        RuleFor(x => x.HireDate)
            .NotEmpty().WithMessage("Data de admissão é obrigatória")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("Data de admissão não pode ser futura");

        RuleFor(x => x.ManagerId)
            .GreaterThan(0).WithMessage("Gerente inválido")
            .When(x => x.ManagerId.HasValue);
    }
}

public class EmployeeUpdateDtoValidator : AbstractValidator<EmployeeUpdateDto>
{
    public EmployeeUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID inválido");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome não pode ter mais de 200 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido")
            .MaximumLength(255).WithMessage("Email não pode ter mais de 255 caracteres");

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("Departamento é obrigatório");

        RuleFor(x => x.PositionId)
            .GreaterThan(0).WithMessage("Cargo é obrigatório");

        RuleFor(x => x.HireDate)
            .NotEmpty().WithMessage("Data de admissão é obrigatória")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("Data de admissão não pode ser futura");

        RuleFor(x => x.ManagerId)
            .GreaterThan(0).WithMessage("Gerente inválido")
            .When(x => x.ManagerId.HasValue);
    }
}
