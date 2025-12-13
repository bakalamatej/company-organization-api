using System.ComponentModel.DataAnnotations;

namespace firmyAPI.DTOs.Project;

public class CreateProjectDto
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = null!;

    [Required, MaxLength(50)]
    public string Code { get; set; } = null!;

    [Required]
    public int DivisionId { get; set; }

    public int? LeaderId { get; set; }
}