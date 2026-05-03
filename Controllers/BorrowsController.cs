using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibaryManagement.Data;
using LibaryManagement.Models;

namespace LibaryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BorrowsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Borrow>>> GetBorrows()
        {
            return await _context.Borrows
                .Include(b => b.Reader).ThenInclude(r => r.User)
                .Include(b => b.BorrowDetails).ThenInclude(bd => bd.Book)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Borrow>> GetBorrow(int id)
        {
            var borrow = await _context.Borrows
                .Include(b => b.Reader).ThenInclude(r => r.User)
                .Include(b => b.BorrowDetails).ThenInclude(bd => bd.Book)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (borrow == null) return NotFound();
            return borrow;
        }

        [HttpPost]
        public async Task<ActionResult<Borrow>> PostBorrow([FromBody] System.Text.Json.JsonElement rawJson)
        {
            try
            {
                // Manual parsing to be extremely robust against casing
                int readerId = 0;
                if (rawJson.TryGetProperty("readerId", out var rId) || rawJson.TryGetProperty("ReaderId", out rId))
                    readerId = rId.GetInt32();

                if (readerId == 0) return BadRequest("ReaderId is required.");

                var borrow = new Borrow
                {
                    ReaderId = readerId,
                    BorrowDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(7),
                    Status = BorrowStatus.Borrowing,
                    BorrowDetails = new List<BorrowDetail>(),
                    TotalAmount = 0
                };

                if (rawJson.TryGetProperty("borrowDate", out var bDate) || rawJson.TryGetProperty("BorrowDate", out bDate))
                    borrow.BorrowDate = bDate.GetDateTime();
                
                if (rawJson.TryGetProperty("dueDate", out var dDate) || rawJson.TryGetProperty("DueDate", out dDate))
                    borrow.DueDate = dDate.GetDateTime();

                decimal pricePerDay = 1500;
                int totalDays = (int)(borrow.DueDate - borrow.BorrowDate).TotalDays;
                if (totalDays <= 0) totalDays = 1;

                if (rawJson.TryGetProperty("borrowDetails", out var details) || rawJson.TryGetProperty("BorrowDetails", out details))
                {
                    foreach (var detailJson in details.EnumerateArray())
                    {
                        int bookId = 0;
                        if (detailJson.TryGetProperty("bookId", out var bkId) || detailJson.TryGetProperty("BookId", out bkId))
                            bookId = bkId.GetInt32();

                        if (bookId > 0)
                        {
                            var book = await _context.Books.FindAsync(bookId);
                            if (book != null && book.Quantity > 0)
                            {
                                book.Quantity -= 1;
                                borrow.BorrowDetails.Add(new BorrowDetail { BookId = bookId });
                                borrow.TotalAmount += (pricePerDay * totalDays);
                            }
                        }
                    }
                }

                _context.Borrows.Add(borrow);
                await _context.SaveChangesAsync();

                var result = await _context.Borrows
                    .Include(b => b.Reader).ThenInclude(r => r.User)
                    .Include(b => b.BorrowDetails).ThenInclude(bd => bd.Book)
                    .FirstOrDefaultAsync(b => b.Id == borrow.Id);

                return CreatedAtAction(nameof(GetBorrow), new { id = borrow.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi hệ thống: {ex.Message}");
            }
        }

        public class BorrowCreateDto
        {
            public int ReaderId { get; set; }
            public DateTime BorrowDate { get; set; }
            public DateTime DueDate { get; set; }
            public BorrowStatus Status { get; set; }
            public List<BorrowDetailCreateDto> BorrowDetails { get; set; } = new();
        }

        public class BorrowDetailCreateDto
        {
            public int BookId { get; set; }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBorrow(int id, Borrow borrow)
        {
            if (id != borrow.Id) return BadRequest();

            // Get the existing record to check for status change
            var existingBorrow = await _context.Borrows
                .Include(b => b.BorrowDetails)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (existingBorrow == null) return NotFound();

            // Check if status is changing to Returned (1)
            if (existingBorrow.Status != BorrowStatus.Returned && borrow.Status == BorrowStatus.Returned)
            {
                if (existingBorrow.BorrowDetails != null)
                {
                    foreach (var detail in existingBorrow.BorrowDetails)
                    {
                        var book = await _context.Books.FindAsync(detail.BookId);
                        if (book != null)
                        {
                            book.Quantity += 1;
                        }
                    }
                }
            }

            // Update properties
            existingBorrow.Status = borrow.Status;
            existingBorrow.DueDate = borrow.DueDate;
            existingBorrow.BorrowDate = borrow.BorrowDate;
            existingBorrow.ReaderId = borrow.ReaderId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BorrowExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBorrow(int id)
        {
            var borrow = await _context.Borrows.FindAsync(id);
            if (borrow == null) return NotFound();
            _context.Borrows.Remove(borrow);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool BorrowExists(int id) => _context.Borrows.Any(e => e.Id == id);
    }
}
