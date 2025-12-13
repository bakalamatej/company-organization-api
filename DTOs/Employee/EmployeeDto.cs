namespace firmyAPI.DTOs.Employee;

public class EmployeeDto
{
    public int Id { get; set; }

    public string? Title { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Phone { get; set; }
    public string Email { get; set; } = null!;

    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = null!;

    public int? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
}
