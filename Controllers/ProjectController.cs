using firmyAPI.Data;
using firmyAPI.DTOs.Project;
using firmyAPI.Models;
using firmyAPI.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace firmyAPI.Controllers;

[ApiController]
[Route("api/companies/{companyId}/divisions/{divisionId}/projects")]
public class ProjectController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IEntityValidator _validator;

    public ProjectController(AppDbContext context, IEntityValidator validator)
    {
        _context = context;
        _validator = validator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAll(int companyId, int divisionId)
    {
        var validation = await _validator.ValidateDivision(divisionId, companyId);
        if (validation != ValidationResult.Success) return NotFound(validation.ToString());

        var projects = await _context.Projects
            .Where(p => p.DivisionId == divisionId)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code,
                DivisionId = p.DivisionId,
                LeaderId = p.LeaderId,
                LeaderName = p.Leader != null ? $"{p.Leader.FirstName} {p.Leader.LastName}" : null
            })
            .ToListAsync();

        return Ok(projects);
    }

    [HttpGet("{projectId}")]
    public async Task<ActionResult<ProjectDto>> GetById(int companyId, int divisionId, int projectId)
    {
        var validation = await _validator.ValidateProject(projectId, divisionId, companyId);
        if (validation != ValidationResult.Success) return NotFound(validation.ToString());

        var p = await _context.Projects
            .Where(p => p.Id == projectId && p.DivisionId == divisionId)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code,
                DivisionId = p.DivisionId,
                LeaderId = p.LeaderId,
                LeaderName = p.Leader != null ? $"{p.Leader.FirstName} {p.Leader.LastName}" : null
            })
            .FirstOrDefaultAsync();

        return Ok(p);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> AddProject(int companyId, int divisionId, [FromBody] CreateProjectDto dto)
    {
        var validation = await _validator.ValidateDivision(divisionId, companyId);
        if (validation != ValidationResult.Success) return NotFound(validation.ToString());

        var leaderValidation = await _validator.ValidateLeader(dto.LeaderId, companyId);
        if (leaderValidation != ValidationResult.Success) return BadRequest(leaderValidation.ToString());

        var project = new Project
        {
            Name = dto.Name,
            Code = dto.Code,
            DivisionId = divisionId,
            LeaderId = dto.LeaderId
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById),
            new { companyId, divisionId, projectId = project.Id },
            new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Code = project.Code,
                DivisionId = divisionId,
                LeaderId = project.LeaderId,
                LeaderName = null
            });
    }

    [HttpPut("{projectId}")]
    public async Task<IActionResult> Update(int companyId, int divisionId, int projectId, [FromBody] UpdateProjectDto dto)
    {
        var validation = await _validator.ValidateProject(projectId, divisionId, companyId);
        if (validation != ValidationResult.Success) return NotFound(validation.ToString());

        var project = await _context.Projects.FindAsync(projectId);

        if (project == null) return NotFound("Department not found.");

        if (!string.IsNullOrWhiteSpace(dto.Name)) project.Name = dto.Name;
        if (!string.IsNullOrWhiteSpace(dto.Code)) project.Code = dto.Code;

        if (dto.LeaderId.HasValue)
        {
            var leaderValidation = await _validator.ValidateLeader(dto.LeaderId, companyId);
            if (leaderValidation != ValidationResult.Success) return BadRequest(leaderValidation.ToString());
            project.LeaderId = dto.LeaderId;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{projectId}")]
    public async Task<IActionResult> Delete(int companyId, int divisionId, int projectId)
    {
        var validation = await _validator.ValidateProject(projectId, divisionId, companyId);
        if (validation != ValidationResult.Success) return NotFound(validation.ToString());

        var project = await _context.Projects.FindAsync(projectId);

        if (project == null) return NotFound("Department not found.");
        
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}