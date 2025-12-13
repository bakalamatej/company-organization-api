using firmyAPI.Data;
using firmyAPI.DTOs.Division;
using firmyAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace firmyAPI.Controllers;
[ApiController]
[Route("api/companies/{companyId}/divisions")]
public class DivisionController : ControllerBase
{
    private readonly AppDbContext _context;
    public DivisionController(AppDbContext context) => _context = context;

    private async Task<bool> CompanyExists(int companyId)
        => await _context.Companies.AnyAsync(c => c.Id == companyId);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DivisionDto>>> GetAll(int companyId)
    {
        if (!await CompanyExists(companyId))
            return NotFound("Company not found.");

        var divisions = await _context.Divisions
            .Where(d => d.CompanyId == companyId)
            .Select(d => new DivisionDto
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                CompanyId = d.CompanyId,
                LeaderId = d.LeaderId,
                LeaderName = d.Leader != null
                    ? d.Leader.FirstName + " " + d.Leader.LastName
                    : null
            })
            .ToListAsync();

        return Ok(divisions);
    }

    [HttpGet("{divisionId}")]
    public async Task<ActionResult<DivisionDto>> GetById(int companyId, int divisionId)
    {
        if (!await CompanyExists(companyId))
            return NotFound("Company not found.");

        var division = await _context.Divisions
            .Where(d => d.Id == divisionId && d.CompanyId == companyId)
            .Select(d => new DivisionDto
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                CompanyId = d.CompanyId,
                LeaderId = d.LeaderId,
                LeaderName = d.Leader != null
                    ? d.Leader.FirstName + " " + d.Leader.LastName
                    : null
            })
            .FirstOrDefaultAsync();

        if (division == null)
            return NotFound();

        return Ok(division);
    }

    [HttpPost]
    public async Task<ActionResult<DivisionDto>> AddDivision(int companyId, [FromBody] CreateDivisionDto dto)
    {
        if (!await CompanyExists(companyId))
            return NotFound("Company not found.");

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
        var division = await _context.Divisions
            .FirstOrDefaultAsync(d => d.Id == divisionId && d.CompanyId == companyId);

        if (division == null)
            return NotFound();

        if (!string.IsNullOrWhiteSpace(dto.Name)) division.Name = dto.Name;
        if (!string.IsNullOrWhiteSpace(dto.Code)) division.Code = dto.Code;
        if (dto.LeaderId.HasValue) division.LeaderId = dto.LeaderId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{divisionId}")]
    public async Task<IActionResult> Delete(int companyId, int divisionId)
    {
        var division = await _context.Divisions
            .FirstOrDefaultAsync(d => d.Id == divisionId && d.CompanyId == companyId);

        if (division == null)
            return NotFound();

        _context.Divisions.Remove(division);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}