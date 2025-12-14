using firmyAPI.Data;
using firmyAPI.DTOs.Department;
using firmyAPI.DTOs.Employee;
using firmyAPI.Models;
using firmyAPI.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace firmyAPI.Controllers;

[ApiController]
[Route("api/companies/{companyId}/divisions/{divisionId}/projects/{projectId}/departments")]
public class DepartmentController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IEntityValidator _validator;

    public DepartmentController(AppDbContext context, IEntityValidator validator)
    {
        _context = context;
        _validator = validator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAll(int companyId, int divisionId, int projectId)
    {
        var validation = await _validator.ValidateProject(projectId, divisionId, companyId);
        if (validation != ValidationResult.Success) return NotFound(validation.ToString());

        var departments = await _context.Departments
            .Where(d => d.ProjectId == projectId)
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                ProjectId = d.ProjectId,
                LeaderId = d.LeaderId,
                LeaderName = d.Leader != null ? $"{d.Leader.FirstName} {d.Leader.LastName}" : null
            })
            .ToListAsync();

        return Ok(departments);
    }

    [HttpGet("{departmentId}")]
    public async Task<ActionResult<DepartmentDto>> GetById(int companyId, int divisionId, int projectId, int departmentId)
    {
        var validation = await _validator.ValidateDepartment(departmentId, projectId, divisionId, companyId);
        if (validation != ValidationResult.Success) return NotFound(validation.ToString());

        var d = await _context.Departments
            .Where(d => d.Id == departmentId && d.ProjectId == projectId)
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                ProjectId = d.ProjectId,
                LeaderId = d.LeaderId,
                LeaderName = d.Leader != null ? $"{d.Leader.FirstName} {d.Leader.LastName}" : null
            })
            .FirstOrDefaultAsync();

        return Ok(d);
    }

    [HttpGet("{departmentId}/employees")]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees(
        int companyId,
        int divisionId,
        int projectId,
        int departmentId)
    {
        var validation = await _validator.ValidateDepartment(
            departmentId,
            projectId,
            divisionId,
            companyId);

        if (validation != ValidationResult.Success)
            return NotFound(validation.ToString());

        var employees = await _context.Employees
            .Where(e => e.DepartmentId == departmentId && e.CompanyId == companyId)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                Title = e.Title,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Phone = e.Phone,
                CompanyId = e.CompanyId,
                DepartmentId = e.DepartmentId
            })
            .ToListAsync();

        return Ok(employees);
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentDto>> AddDepartment(int companyId, int divisionId, int projectId, [FromBody] CreateDepartmentDto dto)
    {
        var validation = await _validator.ValidateProject(projectId, divisionId, companyId);
        if (validation != ValidationResult.Success) return NotFound(validation.ToString());

        var leaderValidation = await _validator.ValidateLeader(dto.LeaderId, companyId);
        if (leaderValidation != ValidationResult.Success) return BadRequest(leaderValidation.ToString());

        var department = new Department
        {
            Name = dto.Name,
            Code = dto.Code,
            ProjectId = projectId,
            LeaderId = dto.LeaderId
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById),
            new { companyId, divisionId, projectId, departmentId = department.Id },
            new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Code = department.Code,
                ProjectId = projectId,
                LeaderId = department.LeaderId,
                LeaderName = null
            });
    }

    [HttpPut("{departmentId}")]
    public async Task<IActionResult> Update(int companyId, int divisionId, int projectId, int departmentId, [FromBody] UpdateDepartmentDto dto)
    {
        var validation = await _validator.ValidateDepartment(departmentId, projectId, divisionId, companyId);
        if (validation != ValidationResult.Success) return NotFound(validation.ToString());

        var department = await _context.Departments.FindAsync(departmentId);

        if (department == null) return NotFound("Department not found.");
            
        if (!string.IsNullOrWhiteSpace(dto.Name)) department.Name = dto.Name;
        if (!string.IsNullOrWhiteSpace(dto.Code)) department.Code = dto.Code;

        if (dto.LeaderId.HasValue)
        {
            var leaderValidation = await _validator.ValidateLeader(dto.LeaderId, companyId);
            if (leaderValidation != ValidationResult.Success) return BadRequest(leaderValidation.ToString());
            department.LeaderId = dto.LeaderId;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{departmentId}")]
    public async Task<IActionResult> Delete(int companyId, int divisionId, int projectId, int departmentId)
    {
        var validation = await _validator.ValidateDepartment(departmentId, projectId, divisionId, companyId);
        if (validation != ValidationResult.Success) return NotFound(validation.ToString());

        var department = await _context.Departments.FindAsync(departmentId);

        if (department == null) return NotFound("Department not found.");

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}