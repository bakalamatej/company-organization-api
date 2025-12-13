namespace firmyAPI.DTOs.Division;

public class DivisionDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;

    public int CompanyId { get; set; }

    public int? LeaderId { get; set; }
    public string? LeaderName { get; set; }
}
