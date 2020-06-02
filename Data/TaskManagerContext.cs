using Microsoft.EntityFrameworkCore;
using TaskManager.Models;

namespace TaskManager.Data
{
    public class TaskManagerContext : DbContext
    {
        public TaskManagerContext(DbContextOptions<TaskManagerContext> opt) : base(opt)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasAlternateKey(c => c.email);
            modelBuilder.Entity<User>().Property(b => b.isAdvancedUser).HasDefaultValue(false);
            modelBuilder.Entity<UserTask>().Property(b => b.deleted).HasDefaultValue(false);

        }

        public DbSet<UserTask> UserTasks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<InvalidToken> InvalidTokens { get; set; }






    }
}