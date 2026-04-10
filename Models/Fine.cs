namespace LibaryManagement.Models
{
    public class Fine
    {
        public int Id { get; set; }

        public int BorrowId { get; set; }
        public virtual Borrow Borrow { get; set; } = null!;

        public decimal Amount { get; set; }
    }
}
