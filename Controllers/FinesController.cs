using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibaryManagement.Data;
using LibaryManagement.Models;

namespace LibaryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FinesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Fine>>> GetFines()
        {
            return await _context.Fines.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Fine>> GetFine(int id)
        {
            var fine = await _context.Fines.FindAsync(id);
            if (fine == null) return NotFound();
            return fine;
        }

        [HttpPost]
        public async Task<ActionResult<Fine>> PostFine(Fine fine)
        {
            _context.Fines.Add(fine);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetFine), new { id = fine.Id }, fine);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutFine(int id, Fine fine)
        {
            if (id != fine.Id) return BadRequest();
            _context.Entry(fine).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!FineExists(id)) return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFine(int id)
        {
            var fine = await _context.Fines.FindAsync(id);
            if (fine == null) return NotFound();
            _context.Fines.Remove(fine);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool FineExists(int id) => _context.Fines.Any(e => e.Id == id);
    }
}
