using firmyAPI.Data;
using firmyAPI.DTOs.Department;
using firmyAPI.DTOs.Division;
using firmyAPI.Models;
using firmyAPI.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace firmyAPI.Controllers;

[ApiController]
[Route("api/companies/{companyId}/divisions")]
public class DivisionController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IEntityValidator _validator;

    public DivisionController(AppDbContext context, IEntityValidator validator)
    {
        _context = context;
        _validator = validator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DivisionDto>>> GetAll(int companyId)
    {
        var validation = await _validator.ValidateCompany(companyId);
        if (validation != ValidationResult.Success) return NotFound(validation.ToString());

        var divisions = await _context.Divisions
            .Where(d => d.CompanyId == companyId)
            .Select(d => new DivisionDto
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                CompanyId = d.CompanyId,
                LeaderId = d.LeaderId,
                LeaderName = d.Leader != null ? $"{d.Leader.FirstName} {d.Leader.LastName}" : null
            })
            .ToListAsync();

        return Ok(divisions);
    }

    [HttpGet("{divisionId}")]
    public async Task<ActionResult<DivisionDto>> GetById(int companyId, int divisionId)
    {
        var validation = await _validator.ValidateDivision(divisionId, companyId);
        if (validation != ValidationResult.Success) return NotFound(validation.ToString());

        var d = await _context.Divisions
            .Where(d => d.Id == divisionId && d.CompanyId == companyId)
            .Select(d => new DivisionDto
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                CompanyId = d.CompanyId,
                LeaderId = d.LeaderId,
                LeaderName = d.Leader != null ? $"{d.Leader.FirstName} {d.Leader.LastName}" : null
            })
            .FirstOrDefaultAsync();

        return Ok(d);
    }

    [HttpGet("{divisionId}/departments")]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDivisionDepartments(
        int companyId,
        int divisionId)
    {
        var validation = await _validator.ValidateDivision(divisionId, companyId);
        if (validation != ValidationResult.Success)
            return NotFound(validation.ToString());

        var departments = await _context.Departments
            .Where(d => d.Project.DivisionId == divisionId)
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
    public async Task<ActionResult<DivisionDto>> AddDivision(int companyId, [FromBody] CreateDivisionDto dto)
    {
        var validation = await _validator.ValidateCompany(companyId);
        if (validation != ValidationResult.Success) return NotFound(validation.ToString());

        var leaderValidation = await _validator.ValidateLeader(dto.LeaderId, companyId);
        if (leaderValidation != ValidationResult.Success) return BadRequest(leaderValidation.ToString());

        var division = new Division
        {
            Name = dto.Name,
            Code = dto.Code,
            CompanyId = companyId,
            LeaderId = dto.LeaderId
        };

        _context.Divisions.Add(division);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById),
            new { companyId, divisionId = division.Id },
            new DivisionDto
            {
                Id = division.Id,
                Name = division.Name,
                Code = division.Code,
                CompanyId = companyId,
                LeaderId = division.LeaderId,
                LeaderName = null
            });
    }

    [HttpPut("{divisionId}")]
    public async Task<IActionResult> Update(int companyId, int divisionId, [FromBody] UpdateDivisionDto dto)
    {
        var validation = await _validator.ValidateDivision(divisionId, companyId);
        if (validation != ValidationResult.Success) return NotFound(validation.ToString());

        var division = await _context.Divisions.FindAsync(divisionId);

        if (division == null) return NotFound("Department not found.");

        if (!string.IsNullOrWhiteSpace(dto.Name)) division.Name = dto.Name;
        if (!string.IsNullOrWhiteSpace(dto.Code)) division.Code = dto.Code;

        if (dto.LeaderId.HasValue)
        {
            var leaderValidation = await _validator.ValidateLeader(dto.LeaderId, companyId);
            if (leaderValidation != ValidationResult.Success) return BadRequest(leaderValidation.ToString());
            division.LeaderId = dto.LeaderId;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{divisionId}")]
    public async Task<IActionResult> Delete(int companyId, int divisionId)
    {
        var validation = await _validator.ValidateDivision(divisionId, companyId);
        if (validation != ValidationResult.Success) return NotFound(validation.ToString());

        var division = await _context.Divisions.FindAsync(divisionId);

        if (division == null) return NotFound("Department not found.");

        _context.Divisions.Remove(division);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}