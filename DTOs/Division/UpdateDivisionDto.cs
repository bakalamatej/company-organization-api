using System.ComponentModel.DataAnnotations;

namespace firmyAPI.DTOs.Division;

public class UpdateDivisionDto
{
    [MaxLength (200)]
    public string? Name { get; set; }

    [MaxLength(50)]
    public string? Code { get; set; }

    public int? LeaderId { get; set; }
}