namespace LibaryManagement.Models
{
    public class Borrow
    {
        public int Id { get; set; }

        public int ReaderId { get; set; }
        public virtual Reader? Reader { get; set; }

        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public BorrowStatus Status { get; set; }
        public decimal TotalAmount { get; set; }

        // Navigation properties
        public virtual ICollection<BorrowDetail>? BorrowDetails { get; set; }
        public virtual ICollection<Fine>? Fines { get; set; }
    }

    public enum BorrowStatus
    {
        Borrowing,
        Returned
    }
}
