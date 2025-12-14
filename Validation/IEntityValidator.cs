namespace firmyAPI.Validation;

public interface IEntityValidator
{
    Task<ValidationResult> ValidateCompany(int companyId);

    Task<ValidationResult> ValidateDivision(int divisionId, int companyId);

    Task<ValidationResult> ValidateProject(int projectId, int divisionId, int companyId);

    Task<ValidationResult> ValidateDepartment(int departmentId, int projectId, int divisionId, int companyId);

    Task<ValidationResult> ValidateEmployee(int employeeId, int companyId);

    Task<ValidationResult> ValidateLeader(int? leaderId, int companyId);
}