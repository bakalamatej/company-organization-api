namespace firmyAPI.DTOs.Department;

public class DepartmentDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;

    public int ProjectId { get; set; }

    public int? LeaderId { get; set; }
    public string? LeaderName { get; set; }
}
