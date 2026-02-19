namespace OrgChart.Application.DTOs;

public class OrgChartNodeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PositionName { get; set; } = string.Empty;
    public string PositionLevel { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public int? ManagerId { get; set; }
    public List<OrgChartNodeDto> Subordinates { get; set; } = new();
}
