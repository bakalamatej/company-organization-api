using System.ComponentModel.DataAnnotations;

namespace firmyAPI.DTOs.Company;

public class CreateCompanyDto
{
    [Required , MaxLength(200)]
    public string Name { get; set; } = null!;

    [Required, MaxLength(50)]
    public string Code { get; set; } = null!;

    public int? LeaderId { get; set; }
}