using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Book_List.Models
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Book> Books { get; set; }

        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     base.OnModelCreating(modelBuilder);
        //     // Additional configurations if needed
        // }

        public class Book
        {
            public int Id { get; set; }
            [MaxLength(100)]
            public string Title { get; set; }
            [MaxLength(100)]
            public string Author { get; set; }
            [MaxLength(50)]
            public string Genre { get; set; }
            [MaxLength(50)]
            public string Subgenre { get; set; }
        }
    }
}