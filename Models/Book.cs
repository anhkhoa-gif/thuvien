namespace LibaryManagement.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Category { get; set; } = "General";
        public string? ImageUrl { get; set; }
        public BookStatus Status { get; set; }

        // Navigation properties
        public virtual ICollection<BorrowDetail>? BorrowDetails { get; set; }
    }

    public enum BookStatus
    {
        Available,
        Borrowed,
        Reserved,
        Lost
    }
}
