using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibaryManagement.Data;
using LibaryManagement.Models;

namespace LibaryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReadersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReadersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reader>>> GetReaders()
        {
            return await _context.Readers.Include(r => r.User).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Reader>> GetReader(int id)
        {
            var reader = await _context.Readers.FindAsync(id);
            if (reader == null) return NotFound();
            return reader;
        }

        [HttpPost]
        public async Task<ActionResult<Reader>> PostReader(ReaderCreateDto readerDto)
        {
            // 1. Check if user exists by ID
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == readerDto.UserId);
            
            // 2. If not found by ID (stale session), try finding by Username if possible
            // Note: We don't have username in DTO yet, but let's assume we want to be safe.
            // Actually, if we don't have username, we can't relink.
            
            if (user == null)
            {
                return BadRequest("Lỗi: Phiên đăng nhập của bạn đã quá cũ. Vui lòng ĐĂNG XUẤT và ĐĂNG NHẬP lại để hệ thống cập nhật mã số mới.");
            }

            // 3. Check if reader already exists
            var existingReader = await _context.Readers
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.UserId == user.Id);
                
            if (existingReader != null)
            {
                return existingReader;
            }
            
            var reader = new Reader
            {
                UserId = user.Id,
                MaxBooks = readerDto.MaxBooks
            };

            _context.Readers.Add(reader);
            await _context.SaveChangesAsync();
            
            var result = await _context.Readers
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == reader.Id);
            
            return CreatedAtAction(nameof(GetReader), new { id = reader.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReader(int id, Reader reader)
        {
            if (id != reader.Id) return BadRequest();
            _context.Entry(reader).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReaderExists(id)) return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReader(int id)
        {
            var reader = await _context.Readers.FindAsync(id);
            if (reader == null) return NotFound();
            _context.Readers.Remove(reader);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool ReaderExists(int id) => _context.Readers.Any(e => e.Id == id);
    }

    public class ReaderCreateDto
    {
        public int UserId { get; set; }
        public int MaxBooks { get; set; }
    }
}
