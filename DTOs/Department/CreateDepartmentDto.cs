using System.ComponentModel.DataAnnotations;

namespace firmyAPI.DTOs.Department;

public class CreateDepartmentDto
{
    [Required , MaxLength(200)]
    public string Name { get; set; } = null!;

    [Required, MaxLength(50)]
    public string Code { get; set; } = null!;

    public int? LeaderId { get; set; }
}