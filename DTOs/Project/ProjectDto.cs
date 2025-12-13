namespace firmyAPI.DTOs.Project;

public class ProjectDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;

    public int DivisionId { get; set; }

    public int? LeaderId { get; set; }
    public string? LeaderName { get; set; }
}
