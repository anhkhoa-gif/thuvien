namespace LibaryManagement.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserStatus Status { get; set; }

        // Navigation properties
        public virtual Reader? Reader { get; set; }
        public virtual ICollection<UserRole>? UserRoles { get; set; }
    }

    public enum UserStatus
    {
        Active,
        Inactive
    }
}
