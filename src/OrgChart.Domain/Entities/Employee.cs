using OrgChart.Domain.Common;
using OrgChart.Domain.ValueObjects;

namespace OrgChart.Domain.Entities;

/// <summary>
/// Representa um colaborador da organização
/// </summary>
public class Employee : BaseEntity
{
    public string Name { get; private set; }
    private string _email;
    public string EmailAddress
    {
        get => _email;
        private set => _email = value;
    }

    public int DepartmentId { get; private set; }
    public int PositionId { get; private set; }
    public DateTime HireDate { get; private set; }
    public int? ManagerId { get; private set; }

    // Navigation properties
    public virtual Department Department { get; private set; }
    public virtual Position Position { get; private set; }
    public virtual Employee? Manager { get; private set; }
    public virtual ICollection<Employee> Subordinates { get; private set; } = new List<Employee>();

    // EF Core precisa de um construtor sem parâmetros
    private Employee() { }

    public Employee(
        string name,
        Email email,
        int departmentId,
        int positionId,
        DateTime hireDate,
        int? managerId = null)
    {
        ValidateName(name);

        Name = name;
        _email = email.Address;
        DepartmentId = departmentId;
        PositionId = positionId;
        HireDate = hireDate;
        ManagerId = managerId;
    }

    public void Update(
        string name,
        Email email,
        int departmentId,
        int positionId,
        DateTime hireDate)
    {
        ValidateName(name);

        Name = name;
        _email = email.Address;
        DepartmentId = departmentId;
        PositionId = positionId;
        HireDate = hireDate;
        MarkAsUpdated();
    }

    public void ChangeManager(int? managerId)
    {
        if (managerId.HasValue && managerId.Value == Id)
            throw new InvalidOperationException("Um colaborador não pode ser gerente de si mesmo");

        ManagerId = managerId;
        MarkAsUpdated();
    }

    public void RemoveManager()
    {
        ManagerId = null;
        MarkAsUpdated();
    }

    public Email GetEmail() => Email.Create(_email);

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome do colaborador é obrigatório", nameof(name));

        if (name.Length > 200)
            throw new ArgumentException("Nome do colaborador não pode ter mais de 200 caracteres", nameof(name));
    }
}
