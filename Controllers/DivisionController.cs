    using firmyAPI.Data;
    using firmyAPI.DTOs.Division;
    using firmyAPI.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    namespace firmyAPI.Controllers;

    [Route("api/companies/{companyId}/divisions")]
    [ApiController]
    public class DivisionController : ControllerBase
    {
        private readonly AppDbContext _context;
        public DivisionController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DivisionDto>>> GetAll(int companyId)
        {
            var divisions = await _context.Divisions
                .Where(d => d.CompanyId == companyId)
                .Include(d => d.Leader)
                .ToListAsync();

            return Ok(divisions.Select(d => new DivisionDto
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                CompanyId = companyId,
                LeaderId = d.LeaderId,
                LeaderName = d.Leader != null ? $"{d.Leader.FirstName} {d.Leader.LastName}" : null
            }));
        }

        [HttpGet("{divisionId}")]
        public async Task<ActionResult<DivisionDto>> GetById(int companyId, int divisionId)
        {
            var d = await _context.Divisions
                .Include(d => d.Leader)
                .Include(d => d.Projects)
                .FirstOrDefaultAsync(d => d.Id == divisionId && d.CompanyId == companyId);

            if (d == null) return NotFound();

            return Ok(new DivisionDto
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                CompanyId = companyId,
                LeaderId = d.LeaderId,
                LeaderName = d.Leader != null ? $"{d.Leader.FirstName} {d.Leader.LastName}" : null
            });
        }

        [HttpPost]
        public async Task<ActionResult<DivisionDto>> AddDivision(int companyId, [FromBody] CreateDivisionDto dto)
        {
            var d = new Division
            {
                Name = dto.Name,
                Code = dto.Code,
                CompanyId = companyId,
                LeaderId = dto.LeaderId
            };
            _context.Divisions.Add(d);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { companyId, divisionId = d.Id }, new DivisionDto
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                CompanyId = companyId,
                LeaderId = d.LeaderId,
                LeaderName = null
            });
        }

        [HttpPut("{divisionId}")]
        public async Task<IActionResult> Update(int companyId, int divisionId, [FromBody] UpdateDivisionDto dto)
        {
            var d = await _context.Divisions.FindAsync(divisionId);
            if (d == null || d.CompanyId != companyId) return NotFound();

            if (!string.IsNullOrEmpty(dto.Name)) d.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Code)) d.Code = dto.Code;
            if (dto.LeaderId.HasValue) d.LeaderId = dto.LeaderId;

            _context.Entry(d).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{divisionId}")]
        public async Task<IActionResult> Delete(int companyId, int divisionId)
        {
            var d = await _context.Divisions.FindAsync(divisionId);
            if (d == null || d.CompanyId != companyId) return NotFound();

            _context.Divisions.Remove(d);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }