    namespace firmyAPI.DTOs.Company;

    public class CompanyDto
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;

        public int? LeaderId { get; set; }
        public string? LeaderName { get; set; } 
    }
