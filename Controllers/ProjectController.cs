using firmyAPI.Data;
using firmyAPI.DTOs.Project;
using firmyAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace firmyAPI.Controllers;

[Route("api/companies/{companyId}/divisions/{divisionId}/projects")]
[ApiController]
public class ProjectController : ControllerBase
{
    private readonly AppDbContext _context;
    public ProjectController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAll(int companyId, int divisionId)
    {
        var projects = await _context.Projects
            .Where(p => p.DivisionId == divisionId)
            .Include(p => p.Leader)
            .ToListAsync();

        return Ok(projects.Select(p => new ProjectDto
        {
            Id = p.Id,
            Name = p.Name,
            Code = p.Code,
            DivisionId = divisionId,
            LeaderId = p.LeaderId,
            LeaderName = p.Leader != null ? $"{p.Leader.FirstName} {p.Leader.LastName}" : null
        }));
    }

    [HttpGet("{projectId}")]
    public async Task<ActionResult<ProjectDto>> GetById(int companyId, int divisionId, int projectId)
    {
        var p = await _context.Projects
            .Include(p => p.Leader)
            .Include(p => p.Departments)
            .FirstOrDefaultAsync(p => p.Id == projectId && p.DivisionId == divisionId);
        if (p == null) return NotFound();

        return Ok(new ProjectDto
        {
            Id = p.Id,
            Name = p.Name,
            Code = p.Code,
            DivisionId = divisionId,
            LeaderId = p.LeaderId,
            LeaderName = p.Leader != null ? $"{p.Leader.FirstName} {p.Leader.LastName}" : null
        });
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> AddProject(int companyId, int divisionId, [FromBody] CreateProjectDto dto)
    {
        var p = new Project
        {
            Name = dto.Name,
            Code = dto.Code,
            DivisionId = divisionId,
            LeaderId = dto.LeaderId
        };
        _context.Projects.Add(p);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { companyId, divisionId, projectId = p.Id }, new ProjectDto
        {
            Id = p.Id,
            Name = p.Name,
            Code = p.Code,
            DivisionId = divisionId,
            LeaderId = p.LeaderId,
            LeaderName = null
        });
    }

    [HttpPut("{projectId}")]
    public async Task<IActionResult> Update(int companyId, int divisionId, int projectId, [FromBody] UpdateProjectDto dto)
    {
        var p = await _context.Projects.FindAsync(projectId);
        if (p == null || p.DivisionId != divisionId) return NotFound();

        if (!string.IsNullOrEmpty(dto.Name)) p.Name = dto.Name;
        if (!string.IsNullOrEmpty(dto.Code)) p.Code = dto.Code;
        if (dto.LeaderId.HasValue) p.LeaderId = dto.LeaderId;

        _context.Entry(p).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{projectId}")]
    public async Task<IActionResult> Delete(int companyId, int divisionId, int projectId)
    {
        var p = await _context.Projects.FindAsync(projectId);
        if (p == null || p.DivisionId != divisionId) return NotFound();

        _context.Projects.Remove(p);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}