namespace LibaryManagement.Models
{
    public class Reader
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        [Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
        public virtual User? User { get; set; }

        public int MaxBooks { get; set; }

        // Navigation properties
        public virtual ICollection<Borrow>? Borrows { get; set; }
    }
}
