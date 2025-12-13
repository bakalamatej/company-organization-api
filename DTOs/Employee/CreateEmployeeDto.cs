using System.ComponentModel.DataAnnotations;

namespace firmyAPI.DTOs.Employee;

public class CreateEmployeeDto
{
    [MaxLength(50)]
    public string? Title { get; set; }

    [Required, MaxLength(100)]
    public string FirstName { get; set; } = null!;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = null!;

    [MaxLength(30)]
    public string? Phone { get; set; }

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = null!;

    public int? DepartmentId { get; set; }
}