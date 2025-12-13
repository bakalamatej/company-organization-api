using firmyAPI.Data;
using firmyAPI.DTOs.Department;
using firmyAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace firmyAPI.Controllers;

[Route("api/companies/{companyId}/divisions/{divisionId}/projects/{projectId}/departments")]
[ApiController]
public class DepartmentController : ControllerBase
{
    private readonly AppDbContext _context;
    public DepartmentController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAll(int companyId, int divisionId, int projectId)
    {
        var departments = await _context.Departments
            .Where(d => d.ProjectId == projectId)
            .Include(d => d.Leader)
            .ToListAsync();

        return Ok(departments.Select(d => new DepartmentDto
        {
            Id = d.Id,
            Name = d.Name,
            Code = d.Code,
            ProjectId = projectId,
            LeaderId = d.LeaderId,
            LeaderName = d.Leader != null ? $"{d.Leader.FirstName} {d.Leader.LastName}" : null
        }));
    }

    [HttpGet("{departmentId}")]
    public async Task<ActionResult<DepartmentDto>> GetById(int companyId, int divisionId, int projectId, int departmentId)
    {
        var d = await _context.Departments
            .Include(d => d.Leader)
            .FirstOrDefaultAsync(d => d.Id == departmentId && d.ProjectId == projectId);
        if (d == null) return NotFound();

        return Ok(new DepartmentDto
        {
            Id = d.Id,
            Name = d.Name,
            Code = d.Code,
            ProjectId = projectId,
            LeaderId = d.LeaderId,
            LeaderName = d.Leader != null ? $"{d.Leader.FirstName} {d.Leader.LastName}" : null
        });
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentDto>> AddDepartment(int companyId, int divisionId, int projectId, [FromBody] CreateDepartmentDto dto)
    {
        var d = new Department
        {
            Name = dto.Name,
            Code = dto.Code,
            ProjectId = projectId,
            LeaderId = dto.LeaderId
        };
        _context.Departments.Add(d);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { companyId, divisionId, projectId, departmentId = d.Id }, new DepartmentDto
        {
            Id = d.Id,
            Name = d.Name,
            Code = d.Code,
            ProjectId = projectId,
            LeaderId = d.LeaderId,
            LeaderName = null
        });
    }

    [HttpPut("{departmentId}")]
    public async Task<IActionResult> Update(int companyId, int divisionId, int projectId, int departmentId, [FromBody] UpdateDepartmentDto dto)
    {
        var d = await _context.Departments.FindAsync(departmentId);
        if (d == null || d.ProjectId != projectId) return NotFound();

        if (!string.IsNullOrEmpty(dto.Name)) d.Name = dto.Name;
        if (!string.IsNullOrEmpty(dto.Code)) d.Code = dto.Code;
        if (dto.LeaderId.HasValue) d.LeaderId = dto.LeaderId;

        _context.Entry(d).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{departmentId}")]
    public async Task<IActionResult> Delete(int companyId, int divisionId, int projectId, int departmentId)
    {
        var d = await _context.Departments.FindAsync(departmentId);
        if (d == null || d.ProjectId != projectId) return NotFound();

        _context.Departments.Remove(d);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}