using firmyAPI.Data;
using firmyAPI.DTOs.Company;
using firmyAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace firmyAPI.Controllers;

[Route("api/companies")]
[ApiController]
public class CompanyController : ControllerBase
{
    private readonly AppDbContext _context;
    public CompanyController(AppDbContext context) => _context = context;

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
        var c = await _context.Companies
            .Include(c => c.Leader)
            .Include(c => c.Divisions)
                .ThenInclude(d => d.Projects)
                    .ThenInclude(p => p.Departments)
            .Include(c => c.Employees)
            .FirstOrDefaultAsync(c => c.Id == companyId);

        if (c == null) return NotFound();

        return Ok(new CompanyDto
        {
            Id = c.Id,
            Name = c.Name,
            Code = c.Code,
            LeaderId = c.LeaderId,
            LeaderName = c.Leader != null ? $"{c.Leader.FirstName} {c.Leader.LastName}" : null
        });
    }

    [HttpPost]
    public async Task<ActionResult<CompanyDto>> Create([FromBody] CreateCompanyDto dto)
    {
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
        if (dto.LeaderId.HasValue) c.LeaderId = dto.LeaderId;

        _context.Entry(c).State = EntityState.Modified;
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