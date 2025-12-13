using System.ComponentModel.DataAnnotations;

namespace firmyAPI.DTOs.Employee;

public class UpdateEmployeeDto
{
    [MaxLength(50)]
    public string? Title { get; set; }

    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    [MaxLength(30)]
    public string? Phone { get; set; }

    [EmailAddress, MaxLength(200)]
    public string? Email { get; set; }

    public int? DepartmentId { get; set; }
}