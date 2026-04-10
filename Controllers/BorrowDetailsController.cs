using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibaryManagement.Data;
using LibaryManagement.Models;

namespace LibaryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowDetailsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BorrowDetailsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BorrowDetail>>> GetBorrowDetails()
        {
            return await _context.BorrowDetails.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BorrowDetail>> GetBorrowDetail(int id)
        {
            var borrowDetail = await _context.BorrowDetails.FindAsync(id);
            if (borrowDetail == null) return NotFound();
            return borrowDetail;
        }

        [HttpPost]
        public async Task<ActionResult<BorrowDetail>> PostBorrowDetail(BorrowDetail borrowDetail)
        {
            _context.BorrowDetails.Add(borrowDetail);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBorrowDetail), new { id = borrowDetail.Id }, borrowDetail);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBorrowDetail(int id, BorrowDetail borrowDetail)
        {
            if (id != borrowDetail.Id) return BadRequest();
            _context.Entry(borrowDetail).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!BorrowDetailExists(id)) return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBorrowDetail(int id)
        {
            var borrowDetail = await _context.BorrowDetails.FindAsync(id);
            if (borrowDetail == null) return NotFound();
            _context.BorrowDetails.Remove(borrowDetail);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool BorrowDetailExists(int id) => _context.BorrowDetails.Any(e => e.Id == id);
    }
}
