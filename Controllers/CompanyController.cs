using firmyAPI.Data;
using firmyAPI.DTOs.Company;
using firmyAPI.DTOs.Department;
using firmyAPI.DTOs.Project;
using firmyAPI.Models;
using firmyAPI.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace firmyAPI.Controllers;

[Route("api/companies")]
[ApiController]
public class CompanyController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IEntityValidator _validator;

    public CompanyController(AppDbContext context, IEntityValidator validator)
    {
        _context = context;
        _validator = validator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAll()
    {
        var companies = await _context.Companies
            .Include(c => c.Leader)
            .Include(c => c.Divisions)
            .Include(c => c.Employees)
            .ToListAsync();

        return Ok(companies.Select(c => new CompanyDto
        {
            Id = c.Id,
            Name = c.Name,
            Code = c.Code,
            LeaderId = c.LeaderId,
            LeaderName = c.Leader != null ? $"{c.Leader.FirstName} {c.Leader.LastName}" : null
        }));
    }

    [HttpGet("{companyId}")]
    public async Task<ActionResult<CompanyDto>> GetById(int companyId)
    {
        var validation = await _validator.ValidateCompany(companyId);
        if (validation != ValidationResult.Success) return NotFound(validation.ToString());

        var c = await _context.Companies
            .Include(c => c.Leader)
            .Include(c => c.Divisions)
                .ThenInclude(d => d.Projects)
                    .ThenInclude(p => p.Departments)
            .Include(c => c.Employees)
            .FirstOrDefaultAsync(c => c.Id == companyId);

        if (c == null) return NotFound("Company not found.");

        return Ok(new CompanyDto
        {
            Id = c.Id,
            Name = c.Name,
            Code = c.Code,
            LeaderId = c.LeaderId,
            LeaderName = c.Leader != null ? $"{c.Leader.FirstName} {c.Leader.LastName}" : null
        });
    }

    [HttpGet("{companyId}/projects")]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetCompanyProjects(int companyId)
    {
        var validation = await _validator.ValidateCompany(companyId);
        if (validation != ValidationResult.Success)
            return NotFound(validation.ToString());

        var projects = await _context.Projects
            .Where(p => p.Division.CompanyId == companyId)
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

    [HttpGet("{companyId}/departments")]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetCompanyDepartments(int companyId)
    {
        var validation = await _validator.ValidateCompany(companyId);
        if (validation != ValidationResult.Success)
            return NotFound(validation.ToString());

        var departments = await _context.Departments
            .Where(d => d.Project.Division.CompanyId == companyId)
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

    [HttpPost]
    public async Task<ActionResult<CompanyDto>> Create([FromBody] CreateCompanyDto dto)
    {
        var leaderValidation = await _validator.ValidateLeader(dto.LeaderId, 0);
        if (leaderValidation != ValidationResult.Success) return BadRequest(leaderValidation.ToString());

        var c = new Company { Name = dto.Name, Code = dto.Code, LeaderId = dto.LeaderId };
        _context.Companies.Add(c);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { companyId = c.Id }, new CompanyDto
        {
            Id = c.Id,
            Name = c.Name,
            Code = c.Code,
            LeaderId = c.LeaderId,
            LeaderName = null
        });
    }

    [HttpPut("{companyId}")]
    public async Task<IActionResult> Update(int companyId, [FromBody] UpdateCompanyDto dto)
    {
        var c = await _context.Companies.FindAsync(companyId);
        if (c == null) return NotFound();

        if (!string.IsNullOrEmpty(dto.Name)) c.Name = dto.Name;
        if (!string.IsNullOrEmpty(dto.Code)) c.Code = dto.Code;

        if (dto.LeaderId.HasValue)
        {
            var leaderValidation = await _validator.ValidateLeader(dto.LeaderId, companyId);
            if (leaderValidation != ValidationResult.Success) return BadRequest(leaderValidation.ToString());
            c.LeaderId = dto.LeaderId;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{companyId}")]
    public async Task<IActionResult> Delete(int companyId)
    {
        var c = await _context.Companies.FindAsync(companyId);
        if (c == null) return NotFound();

        _context.Companies.Remove(c);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}