using System.ComponentModel.DataAnnotations;

namespace firmyAPI.DTOs.Department;

public class UpdateDepartmentDto
{
    [MaxLength (200)]
    public string? Name { get; set; }

    [MaxLength(50)]
    public string? Code { get; set; }

    public int? LeaderId { get; set; }
}