using Microsoft.EntityFrameworkCore;
using TestWebApplication.models;

namespace TestWebApplication.Controllers
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<TreeNode> TreeNodes { get; set; }
        public DbSet<ExceptionLog> ExceptionLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TreeNode>()
                .HasOne(n => n.Parent)
                .WithMany(n => n.Children)
                .HasForeignKey(n => n.ParentId);

            modelBuilder.Entity<TreeNode>()
                .HasIndex(n => new { n.TreeId, n.Name })
                .IsUnique();
        }
    }
}
