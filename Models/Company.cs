using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace firmyAPI.Models;

public class Company
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = null!;

    [Required, MaxLength(50)]
    public string Code { get; set; } = null!;

    public int? LeaderId { get; set; }

    [ForeignKey(nameof(LeaderId))]
    public Employee? Leader { get; set; }

    public List<Division> Divisions { get; set; } = [];
    public List<Employee> Employees { get; set; } = []; 
}
