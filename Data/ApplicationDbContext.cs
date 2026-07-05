using ECommerceMVC.Models;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Models;
namespace TaskManagementSystem.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
       : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.AssignedUser)
                .WithMany()
                .HasForeignKey(t => t.AssignedUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Notification>()
               .HasOne(n => n.Task)
               .WithMany()
               .HasForeignKey(n => n.TaskId)
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Notification>()
               .HasOne(n => n.User)
               .WithMany()
               .HasForeignKey(n => n.UserId)
               .OnDelete(DeleteBehavior.NoAction);
        }

        public DbSet<User> Users => Set<User>();

        public DbSet<Project> Projects => Set<Project>();

        public DbSet<TaskItem> Tasks => Set<TaskItem>();

        public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();

        public DbSet<Notification> Notifications =>Set<Notification>();

        public DbSet<TaskAttachment> TaskAttachments => Set<TaskAttachment>();
    }
}
