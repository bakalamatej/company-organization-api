using firmyAPI.Data;
using firmyAPI.DTOs.Department;
using firmyAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace firmyAPI.Controllers;

[ApiController]
[Route("api/companies/{companyId}/divisions/{divisionId}/projects/{projectId}/departments")]
public class DepartmentController : ControllerBase
{
    private readonly AppDbContext _context;
    public DepartmentController(AppDbContext context) => _context = context;

    private async Task<bool> ProjectBelongsToDivision(
        int companyId, int divisionId, int projectId)
    {
        return await _context.Projects.AnyAsync(p =>
            p.Id == projectId &&
            p.DivisionId == divisionId &&
            p.Division.CompanyId == companyId);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAll(
        int companyId, int divisionId, int projectId)
    {
        if (!await ProjectBelongsToDivision(companyId, divisionId, projectId))
            return NotFound("Project does not belong to division.");

        var departments = await _context.Departments
            .Where(d => d.ProjectId == projectId)
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                ProjectId = d.ProjectId,
                LeaderId = d.LeaderId,
                LeaderName = d.Leader != null
                    ? d.Leader.FirstName + " " + d.Leader.LastName
                    : null
            })
            .ToListAsync();

        return Ok(departments);
    }

    [HttpGet("{departmentId}")]
    public async Task<ActionResult<DepartmentDto>> GetById(
        int companyId, int divisionId, int projectId, int departmentId)
    {
        if (!await ProjectBelongsToDivision(companyId, divisionId, projectId))
            return NotFound("Project does not belong to division.");

        var department = await _context.Departments
            .Where(d => d.Id == departmentId && d.ProjectId == projectId)
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                ProjectId = d.ProjectId,
                LeaderId = d.LeaderId,
                LeaderName = d.Leader != null
                    ? d.Leader.FirstName + " " + d.Leader.LastName
                    : null
            })
            .FirstOrDefaultAsync();

        if (department == null)
            return NotFound();

        return Ok(department);
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentDto>> AddDepartment(
        int companyId, int divisionId, int projectId,
        [FromBody] CreateDepartmentDto dto)
    {
        if (!await ProjectBelongsToDivision(companyId, divisionId, projectId))
            return NotFound("Project does not belong to division.");

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
    public async Task<IActionResult> Update(
        int companyId, int divisionId, int projectId, int departmentId,
        [FromBody] UpdateDepartmentDto dto)
    {
        if (!await ProjectBelongsToDivision(companyId, divisionId, projectId))
            return NotFound("Project does not belong to division.");

        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == departmentId && d.ProjectId == projectId);

        if (department == null)
            return NotFound();

        if (!string.IsNullOrWhiteSpace(dto.Name)) department.Name = dto.Name;
        if (!string.IsNullOrWhiteSpace(dto.Code)) department.Code = dto.Code;
        if (dto.LeaderId.HasValue) department.LeaderId = dto.LeaderId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{departmentId}")]
    public async Task<IActionResult> Delete(
        int companyId, int divisionId, int projectId, int departmentId)
    {
        if (!await ProjectBelongsToDivision(companyId, divisionId, projectId))
            return NotFound("Project does not belong to division.");

        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == departmentId && d.ProjectId == projectId);

        if (department == null)
            return NotFound();

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}