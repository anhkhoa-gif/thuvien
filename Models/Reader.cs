namespace LibaryManagement.Models
{
    public class Reader
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;

        public int MaxBooks { get; set; }

        // Navigation properties
        public virtual ICollection<Borrow>? Borrows { get; set; }
    }
}
