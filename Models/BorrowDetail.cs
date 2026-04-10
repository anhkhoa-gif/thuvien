namespace LibaryManagement.Models
{
    public class BorrowDetail
    {
        public int Id { get; set; }

        public int BorrowId { get; set; }
        public virtual Borrow Borrow { get; set; } = null!;

        public int BookId { get; set; }
        public virtual Book Book { get; set; } = null!;
    }
}
