using Microsoft.EntityFrameworkCore;
using ZamowKsiazke.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ZamowKsiazke.Data
{
    public class ZamowKsiazkeContext : IdentityDbContext<DefaultUser>
    {
        public ZamowKsiazkeContext(DbContextOptions<ZamowKsiazkeContext> options)
            : base(options)
        {
            Book = Set<Book>();
            CartItems = Set<CartItem>();
            Orders = Set<Order>();
            OrderItems = Set<OrderItem>();
            UserActivities = Set<UserActivity>();
            BookBorrowings = Set<BookBorrowing>();
        }

        public virtual DbSet<Book> Book { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<UserActivity> UserActivities { get; set; }
        public virtual DbSet<BookBorrowing> BookBorrowings { get; set; }
    }
}
