namespace LibaryManagement.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = string.Empty; // reader, librarian, admin
        public string Description { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<UserRole>? UserRoles { get; set; }
    }
}
