using LibaryManagement.Models;

namespace LibaryManagement.Data
{
    public static class DbSeeder
    {
        public static void SeedData(AppDbContext context)
        {
            // Seed Roles
            if (!context.Roles.Any())
            {
                var role = new Role { RoleName = "reader", Description = "Library Reader" };
                context.Roles.Add(role);
                context.SaveChanges();
            }

            // Seed Users
            if (!context.Users.Any())
            {
                var user1 = new User
                {
                    Username = "nguyenvana",
                    PasswordHash = "hashed_password", // just dummy
                    FullName = "Nguyen Van A",
                    Email = "vana@example.com",
                    Status = UserStatus.Active
                };
                context.Users.Add(user1);
                context.SaveChanges();

                var role = context.Roles.FirstOrDefault(r => r.RoleName == "reader");
                if (role != null)
                {
                    context.UserRoles.Add(new UserRole { UserId = user1.Id, RoleId = role.Id });
                }

                // Seed Reader
                var reader = new Reader
                {
                    UserId = user1.Id,
                    MaxBooks = 5
                };
                context.Readers.Add(reader);
                context.SaveChanges();
            }

            // Seed Books
            if (!context.Books.Any())
            {
                var books = new List<Book>
                {
                    new Book { Title = "Clean Code", Author = "Robert C. Martin", Quantity = 10, Status = BookStatus.Available },
                    new Book { Title = "Design Patterns", Author = "Erich Gamma", Quantity = 5, Status = BookStatus.Available },
                    new Book { Title = "C# in Depth", Author = "Jon Skeet", Quantity = 3, Status = BookStatus.Available }
                };
                context.Books.AddRange(books);
                context.SaveChanges();
            }

            // Seed Borrows for testing
            if (!context.Borrows.Any())
            {
                var reader = context.Readers.FirstOrDefault();
                var book = context.Books.FirstOrDefault();

                if (reader != null && book != null)
                {
                    var borrow = new Borrow
                    {
                        ReaderId = reader.Id,
                        BorrowDate = DateTime.Now,
                        DueDate = DateTime.Now.AddDays(14),
                        Status = BorrowStatus.Borrowing
                    };
                    context.Borrows.Add(borrow);
                    context.SaveChanges();

                    context.BorrowDetails.Add(new BorrowDetail
                    {
                        BorrowId = borrow.Id,
                        BookId = book.Id
                    });
                    context.SaveChanges();
                }
            }
        }
    }
}
