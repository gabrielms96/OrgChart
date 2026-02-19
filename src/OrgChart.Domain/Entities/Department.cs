using OrgChart.Domain.Common;

namespace OrgChart.Domain.Entities;

/// <summary>
/// Representa um departamento ou unidade organizacional
/// </summary>
public class Department : BaseEntity
{
    public string Name { get; private set; }
    public string? Code { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation property
    public virtual ICollection<Employee> Employees { get; private set; } = new List<Employee>();

    // EF Core precisa de um construtor sem parâmetros
    private Department() { }

    public Department(string name, string? code = null, bool isActive = true)
    {
        ValidateName(name);
        ValidateCode(code);

        Name = name;
        Code = code?.Trim();
        IsActive = isActive;
    }

    public void Update(string name, string? code, bool isActive)
    {
        ValidateName(name);
        ValidateCode(code);

        Name = name;
        Code = code?.Trim();
        IsActive = isActive;
        MarkAsUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        MarkAsUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome do departamento é obrigatório", nameof(name));

        if (name.Length > 200)
            throw new ArgumentException("Nome do departamento não pode ter mais de 200 caracteres", nameof(name));
    }

    private void ValidateCode(string? code)
    {
        if (code != null && code.Length > 50)
            throw new ArgumentException("Código do departamento não pode ter mais de 50 caracteres", nameof(code));
    }
}
