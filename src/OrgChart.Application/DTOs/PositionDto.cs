using OrgChart.Domain.Enums;

namespace OrgChart.Application.DTOs;

public class PositionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public EPositionLevel Level { get; set; }
    public string LevelDescription { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int EmployeeCount { get; set; }
}

public class PositionCreateDto
{
    public string Name { get; set; } = string.Empty;
    public EPositionLevel Level { get; set; }
    public bool IsActive { get; set; } = true;
}

public class PositionUpdateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public EPositionLevel Level { get; set; }
    public bool IsActive { get; set; }
}
