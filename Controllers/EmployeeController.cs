using firmyAPI.Data;
using firmyAPI.DTOs.Employee;
using firmyAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace firmyAPI.Controllers;

[ApiController]
[Route("api/companies/{companyId}/employees")]
public class EmployeeController : ControllerBase
{
    private readonly AppDbContext _context;
    public EmployeeController(AppDbContext context) => _context = context;

    private async Task<bool> CompanyExists(int companyId)
        => await _context.Companies.AnyAsync(c => c.Id == companyId);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll(int companyId)
    {
        if (!await CompanyExists(companyId))
            return NotFound("Company not found.");

        var employees = await _context.Employees
            .Where(e => e.CompanyId == companyId)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                CompanyId = e.CompanyId,
                CompanyName = e.Company.Name,
                DepartmentId = e.DepartmentId,
                Title = e.Title,
                Phone = e.Phone,
                Email = e.Email
            })
            .ToListAsync();

        return Ok(employees);
    }

    [HttpGet("{employeeId}")]
    public async Task<ActionResult<EmployeeDto>> GetById(int companyId, int employeeId)
    {
        if (!await CompanyExists(companyId))
            return NotFound("Company not found.");

        var employee = await _context.Employees
            .Where(e => e.Id == employeeId && e.CompanyId == companyId)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                CompanyId = e.CompanyId,
                CompanyName = e.Company.Name,
                DepartmentId = e.DepartmentId,
                Title = e.Title,
                Phone = e.Phone,
                Email = e.Email
            })
            .FirstOrDefaultAsync();

        if (employee == null)
            return NotFound();

        return Ok(employee);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create(int companyId, [FromBody] CreateEmployeeDto dto)
    {
        if (!await CompanyExists(companyId))
            return NotFound("Company not found.");

        var employee = new Employee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            CompanyId = companyId,
            DepartmentId = dto.DepartmentId,
            Title = dto.Title,
            Phone = dto.Phone,
            Email = dto.Email
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        var company = await _context.Companies.FindAsync(companyId);

        return CreatedAtAction(nameof(GetById),
            new { companyId, employeeId = employee.Id },
            new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                CompanyId = companyId,
                CompanyName = company!.Name,
                DepartmentId = employee.DepartmentId,
                Title = employee.Title,
                Phone = employee.Phone,
                Email = employee.Email
            });
    }

    [HttpPut("{employeeId}")]
    public async Task<IActionResult> Update(int companyId, int employeeId, [FromBody] UpdateEmployeeDto dto)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == employeeId && e.CompanyId == companyId);

        if (employee == null)
            return NotFound();

        if (!string.IsNullOrWhiteSpace(dto.FirstName)) employee.FirstName = dto.FirstName;
        if (!string.IsNullOrWhiteSpace(dto.LastName)) employee.LastName = dto.LastName;
        if (!string.IsNullOrWhiteSpace(dto.Title)) employee.Title = dto.Title;
        if (!string.IsNullOrWhiteSpace(dto.Phone)) employee.Phone = dto.Phone;
        if (!string.IsNullOrWhiteSpace(dto.Email)) employee.Email = dto.Email;
        if (dto.DepartmentId.HasValue) employee.DepartmentId = dto.DepartmentId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{employeeId}")]
    public async Task<IActionResult> Delete(int companyId, int employeeId)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == employeeId && e.CompanyId == companyId);

        if (employee == null)
            return NotFound();

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}