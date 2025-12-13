using firmyAPI.Data;
using firmyAPI.DTOs.Project;
using firmyAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace firmyAPI.Controllers;

[ApiController]
[Route("api/companies/{companyId}/divisions/{divisionId}/projects")]
public class ProjectController : ControllerBase
{
    private readonly AppDbContext _context;
    public ProjectController(AppDbContext context) => _context = context;

    private async Task<bool> DivisionBelongsToCompany(int companyId, int divisionId)
    {
        return await _context.Divisions
            .AnyAsync(d => d.Id == divisionId && d.CompanyId == companyId);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAll(int companyId, int divisionId)
    {
        if (!await DivisionBelongsToCompany(companyId, divisionId))
            return NotFound("Division does not belong to company.");

        var projects = await _context.Projects
            .Where(p => p.DivisionId == divisionId)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code,
                DivisionId = p.DivisionId,
                LeaderId = p.LeaderId,
                LeaderName = p.Leader != null
                    ? p.Leader.FirstName + " " + p.Leader.LastName
                    : null
            })
            .ToListAsync();

        return Ok(projects);
    }

    [HttpGet("{projectId}")]
    public async Task<ActionResult<ProjectDto>> GetById(
        int companyId,
        int divisionId,
        int projectId)
    {
        if (!await DivisionBelongsToCompany(companyId, divisionId))
            return NotFound("Division does not belong to company.");

        var project = await _context.Projects
            .Where(p => p.Id == projectId && p.DivisionId == divisionId)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code,
                DivisionId = p.DivisionId,
                LeaderId = p.LeaderId,
                LeaderName = p.Leader != null
                    ? p.Leader.FirstName + " " + p.Leader.LastName
                    : null
            })
            .FirstOrDefaultAsync();

        if (project == null)
            return NotFound();

        return Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> AddProject(
        int companyId,
        int divisionId,
        [FromBody] CreateProjectDto dto)
    {
        if (!await DivisionBelongsToCompany(companyId, divisionId))
            return NotFound("Division does not belong to company.");

        var p = new Project
        {
            Name = dto.Name,
            Code = dto.Code,
            DivisionId = divisionId,
            LeaderId = dto.LeaderId
        };

        _context.Projects.Add(p);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { companyId, divisionId, projectId = p.Id },
            new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code,
                DivisionId = p.DivisionId,
                LeaderId = p.LeaderId,
                LeaderName = null
            });
    }

    [HttpPut("{projectId}")]
    public async Task<IActionResult> Update(
        int companyId,
        int divisionId,
        int projectId,
        [FromBody] UpdateProjectDto dto)
    {
        if (!await DivisionBelongsToCompany(companyId, divisionId))
            return NotFound("Division does not belong to company.");

        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == projectId && p.DivisionId == divisionId);

        if (project == null)
            return NotFound();

        if (!string.IsNullOrWhiteSpace(dto.Name))
            project.Name = dto.Name;

        if (!string.IsNullOrWhiteSpace(dto.Code))
            project.Code = dto.Code;

        if (dto.LeaderId.HasValue)
            project.LeaderId = dto.LeaderId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{projectId}")]
    public async Task<IActionResult> Delete(
        int companyId,
        int divisionId,
        int projectId)
    {
        if (!await DivisionBelongsToCompany(companyId, divisionId))
            return NotFound("Division does not belong to company.");

        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == projectId && p.DivisionId == divisionId);

        if (project == null)
            return NotFound();

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}