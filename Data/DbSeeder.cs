using LibaryManagement.Models;
using System.Collections.Generic;
using System.Linq;

namespace LibaryManagement.Data
{
    public static class DbSeeder
    {
        public static void SeedData(AppDbContext context)
        {
            // Seed Roles
            if (!context.Roles.Any(r => r.RoleName == "reader"))
            {
                context.Roles.Add(new Role { RoleName = "reader", Description = "Library Reader" });
                context.SaveChanges();
            }
            if (!context.Roles.Any(r => r.RoleName == "admin"))
            {
                context.Roles.Add(new Role { RoleName = "admin", Description = "Library Administrator" });
                context.SaveChanges();
            }

            // Seed Users
            if (!context.Users.Any(u => u.Username == "nguyenvana"))
            {
                var user1 = new User
                {
                    Username = "nguyenvana",
                    PasswordHash = "hashed_password",
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
                    context.SaveChanges();
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

            if (!context.Users.Any(u => u.Username == "admin"))
            {
                var adminUser = new User
                {
                    Username = "admin",
                    PasswordHash = "admin",
                    FullName = "System Admin",
                    Email = "admin@openboox.vn",
                    Status = UserStatus.Active
                };
                context.Users.Add(adminUser);
                context.SaveChanges();

                var adminRole = context.Roles.FirstOrDefault(r => r.RoleName == "admin");
                if (adminRole != null)
                {
                    context.UserRoles.Add(new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id });
                    context.SaveChanges();
                }
            }


            // Expand Books and Categories
            var existingTitles = context.Books.Select(b => b.Title).ToList();
            var newBooks = new List<Book>
            {
                // Kinh doanh & Tài chính
                new Book { Title = "Nhà Đầu Tư Thông Minh", Author = "Benjamin Graham", Quantity = 10, Category = "Kinh doanh", ImageUrl = "https://images.unsplash.com/photo-1611974717525-538a16db8c61?auto=format&fit=crop&q=80&w=400", Status = BookStatus.Available },
                new Book { Title = "Từ Tốt Đến Vĩ Đại", Author = "Jim Collins", Quantity = 7, Category = "Kinh doanh", ImageUrl = "https://images.unsplash.com/photo-1460925895917-afdab827c52f?auto=format&fit=crop&q=80&w=400", Status = BookStatus.Available },
                
                // Khoa học & Công nghệ
                new Book { Title = "Lược Sử Thời Gian", Author = "Stephen Hawking", Quantity = 5, Category = "Khoa học", ImageUrl = "https://images.unsplash.com/photo-1614728263952-84ea256f9679?auto=format&fit=crop&q=80&w=400", Status = BookStatus.Available },
                new Book { Title = "Clean Code", Author = "Robert C. Martin", Quantity = 15, Category = "Công nghệ", ImageUrl = "https://images.unsplash.com/photo-1517694712202-14dd9538aa97?auto=format&fit=crop&q=80&w=400", Status = BookStatus.Available },

                // Văn học & Nghệ thuật
                new Book { Title = "Số Đỏ", Author = "Vũ Trọng Phụng", Quantity = 20, Category = "Văn học Việt Nam", ImageUrl = "https://images.unsplash.com/photo-1544947950-fa07a98d237f?auto=format&fit=crop&q=80&w=400", Status = BookStatus.Available },
                (new Book { Title = "Truyện Kiều", Author = "Nguyễn Du", Quantity = 25, Category = "Văn học Việt Nam", ImageUrl = "https://images.unsplash.com/photo-1532012197367-27e13d39589d?auto=format&fit=crop&q=80&w=400", Status = BookStatus.Available }),
                new Book { Title = "Mắt Biếc", Author = "Nguyễn Nhật Ánh", Quantity = 30, Category = "Văn học Việt Nam", ImageUrl = "https://images.unsplash.com/photo-1553729459-efe14ef6055d?auto=format&fit=crop&q=80&w=400", Status = BookStatus.Available },

                // Lịch sử & Văn hóa
                new Book { Title = "Guns, Germs, and Steel", Author = "Jared Diamond", Quantity = 6, Category = "Lịch sử", ImageUrl = "https://images.unsplash.com/photo-1505664194779-8beaceb93744?auto=format&fit=crop&q=80&w=400", Status = BookStatus.Available },
                new Book { Title = "Việt Nam Sử Lược", Author = "Trần Trọng Kim", Quantity = 12, Category = "Lịch sử", ImageUrl = "https://images.unsplash.com/photo-1589923188900-85dae523342b?auto=format&fit=crop&q=80&w=400", Status = BookStatus.Available },

                // Thiếu nhi
                new Book { Title = "Dế Mèn Phiêu Lưu Ký", Author = "Tô Hoài", Quantity = 40, Category = "Thiếu nhi", ImageUrl = "https://images.unsplash.com/photo-1535905557558-afc4899a21ad?auto=format&fit=crop&q=80&w=400", Status = BookStatus.Available },
                new Book { Title = "Hoàng Tử Bé", Author = "Antoine de Saint-Exupéry", Quantity = 15, Category = "Thiếu nhi", ImageUrl = "https://images.unsplash.com/photo-1589859762194-eaae75c61f64?auto=format&fit=crop&q=80&w=400", Status = BookStatus.Available },

                // Kỹ năng sống
                new Book { Title = "Đánh Thức Con Người Phi Thường Trong Bạn", Author = "Anthony Robbins", Quantity = 18, Category = "Kỹ năng sống", ImageUrl = "https://images.unsplash.com/photo-1519681393784-d120267933ba?auto=format&fit=crop&q=80&w=400", Status = BookStatus.Available }
            };

            foreach (var b in newBooks)
            {
                if (!existingTitles.Contains(b.Title))
                {
                    context.Books.Add(b);
                }
            }
            context.SaveChanges();

            // Seed Borrows for testing
            if (!context.Borrows.Any())
            {
                var firstReader = context.Readers.FirstOrDefault();
                var firstBook = context.Books.FirstOrDefault();

                if (firstReader != null && firstBook != null)
                {
                    var borrow = new Borrow
                    {
                        ReaderId = firstReader.Id,
                        BorrowDate = DateTime.Now,
                        DueDate = DateTime.Now.AddDays(14),
                        Status = BorrowStatus.Borrowing
                    };
                    context.Borrows.Add(borrow);
                    context.SaveChanges();

                    context.BorrowDetails.Add(new BorrowDetail
                    {
                        BorrowId = borrow.Id,
                        BookId = firstBook.Id
                    });
                    context.SaveChanges();
                }
            }
        }
    }
}
