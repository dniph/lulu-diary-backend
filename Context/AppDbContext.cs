using lulu_diary_backend.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace lulu_diary_backend.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }

        public DbSet<Diary> Diaries { get; set; }
    }
}
