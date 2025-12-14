using firmyAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace firmyAPI.Validation;

public enum ValidationResult
{
    Success,
    CompanyNotFound,
    DivisionNotFound,
    DivisionNotInCompany,
    ProjectNotFound,
    ProjectNotInDivision,
    DepartmentNotFound,
    DepartmentNotInProject,
    EmployeeNotInCompany,
    InvalidLeader
}

public class EntityValidator : IEntityValidator
{
    private readonly AppDbContext _context;

    public EntityValidator(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ValidationResult> ValidateCompany(int companyId)
    {
        if (!await _context.Companies.AnyAsync(c => c.Id == companyId))
            return ValidationResult.CompanyNotFound;

        return ValidationResult.Success;
    }

    public async Task<ValidationResult> ValidateDivision(int divisionId, int companyId)
    {
        var division = await _context.Divisions
            .FirstOrDefaultAsync(d => d.Id == divisionId);

        if (division == null) return ValidationResult.DivisionNotFound;
        if (division.CompanyId != companyId) return ValidationResult.DivisionNotInCompany;

        return ValidationResult.Success;
    }

    public async Task<ValidationResult> ValidateProject(int projectId, int divisionId, int companyId)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null) return ValidationResult.ProjectNotFound;
        if (project.DivisionId != divisionId) return ValidationResult.ProjectNotInDivision;

        return await ValidateDivision(divisionId, companyId);
    }

    public async Task<ValidationResult> ValidateDepartment(int departmentId, int projectId, int divisionId, int companyId)
    {
        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == departmentId);

        if (department == null) return ValidationResult.DepartmentNotFound;
        if (department.ProjectId != projectId) return ValidationResult.DepartmentNotInProject;

        return await ValidateProject(projectId, divisionId, companyId);
    }

    public async Task<ValidationResult> ValidateEmployee(int employeeId, int companyId)
    {
        var exists = await _context.Employees
            .AnyAsync(e => e.Id == employeeId && e.CompanyId == companyId);

        return exists ? ValidationResult.Success : ValidationResult.EmployeeNotInCompany;
    }

    public async Task<ValidationResult> ValidateLeader(int? leaderId, int companyId)
    {
        if (!leaderId.HasValue) return ValidationResult.Success;

        var result = await ValidateEmployee(leaderId.Value, companyId);
        return result == ValidationResult.Success ? ValidationResult.Success : ValidationResult.InvalidLeader;
    }
}