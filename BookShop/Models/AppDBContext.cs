using Microsoft.EntityFrameworkCore;

namespace BookShop.Models
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }
        public DbSet<User> users { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<Supplier> suppliers { get; set; }
        public DbSet<Book> books { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<OrderDetail> orderDetails { get; set; }
    }
}
