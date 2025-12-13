using firmyAPI.Data;
using firmyAPI.DTOs.Employee;
using firmyAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace firmyAPI.Controllers;

[Route("api/companies/{companyId}/employees")]
[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly AppDbContext _context;
    public EmployeeController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll(int companyId)
    {
        var employees = await _context.Employees
            .Where(e => e.CompanyId == companyId)
            .Include(e => e.Company)
            .ToListAsync();

        return Ok(employees.Select(e => new EmployeeDto
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            CompanyId = companyId,
            CompanyName = e.Company.Name,
            DepartmentId = e.DepartmentId
        }));
    }

    [HttpGet("{employeeId}")]
    public async Task<ActionResult<EmployeeDto>> GetById(int companyId, int employeeId)
    {
        var e = await _context.Employees
            .Include(e => e.Company)
            .FirstOrDefaultAsync(e => e.Id == employeeId && e.CompanyId == companyId);

        if (e == null) return NotFound();

        return Ok(new EmployeeDto
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            CompanyId = companyId,
            CompanyName = e.Company.Name,
            DepartmentId = e.DepartmentId
        });
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create(int companyId, [FromBody] CreateEmployeeDto dto)
    {
        var e = new Employee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            CompanyId = companyId,
            DepartmentId = dto.DepartmentId,
            Title = dto.Title,
            Phone = dto.Phone,
            Email = dto.Email
        };

        _context.Employees.Add(e);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { companyId, employeeId = e.Id }, new EmployeeDto
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            CompanyId = companyId,
            CompanyName = e.Company.Name,
            DepartmentId = e.DepartmentId,
            Title = e.Title,
            Phone = e.Phone,
            Email = e.Email
        });
    }

    [HttpPut("{employeeId}")]
    public async Task<IActionResult> Update(int companyId, int employeeId, [FromBody] UpdateEmployeeDto dto)
    {
        var e = await _context.Employees.FindAsync(employeeId);
        if (e == null || e.CompanyId != companyId) return NotFound();

        if (!string.IsNullOrEmpty(dto.FirstName)) e.FirstName = dto.FirstName;
        if (!string.IsNullOrEmpty(dto.LastName)) e.LastName = dto.LastName;
        if (!string.IsNullOrEmpty(dto.Title)) e.Title = dto.Title;
        if (!string.IsNullOrEmpty(dto.Phone)) e.Phone = dto.Phone;
        if (!string.IsNullOrEmpty(dto.Email)) e.Email = dto.Email;
        if (dto.DepartmentId.HasValue) e.DepartmentId = dto.DepartmentId;

        _context.Entry(e).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{employeeId}")]
    public async Task<IActionResult> Delete(int companyId, int employeeId)
    {
        var e = await _context.Employees.FindAsync(employeeId);
        if (e == null || e.CompanyId != companyId) return NotFound();

        _context.Employees.Remove(e);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}