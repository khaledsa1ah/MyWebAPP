using Microsoft.EntityFrameworkCore;

namespace MyWebAPP.Models;

public class LibraryContext : DbContext
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Category> Categories { get; set; }

    public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure relationships, constraints, etc.
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Author)
            .WithMany()
            .HasForeignKey(b => b.AuthorId);

        modelBuilder.Entity<Book>()
            .HasOne(b => b.Category)
            .WithMany()
            .HasForeignKey(b => b.CategoryId);

        // Add other configurations as needed
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<UserPermission>().ToTable("UserPermissions").HasKey(x=> new {x.UserID, x.PermissionID});

        base.OnModelCreating(modelBuilder);
    }
}