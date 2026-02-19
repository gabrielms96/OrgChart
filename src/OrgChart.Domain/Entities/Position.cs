using OrgChart.Domain.Common;
using OrgChart.Domain.Enums;

namespace OrgChart.Domain.Entities;

/// <summary>
/// Representa um cargo na organização
/// </summary>
public class Position : BaseEntity
{
    public string Name { get; private set; }
    public EPositionLevel Level { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation property
    public virtual ICollection<Employee> Employees { get; private set; } = new List<Employee>();

    // EF Core precisa de um construtor sem parâmetros
    private Position() { }

    public Position(string name, EPositionLevel level, bool isActive = true)
    {
        ValidateName(name);

        Name = name;
        Level = level;
        IsActive = isActive;
    }

    public void Update(string name, EPositionLevel level, bool isActive)
    {
        ValidateName(name);

        Name = name;
        Level = level;
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
            throw new ArgumentException("Nome do cargo é obrigatório", nameof(name));

        if (name.Length > 200)
            throw new ArgumentException("Nome do cargo não pode ter mais de 200 caracteres", nameof(name));
    }
}
