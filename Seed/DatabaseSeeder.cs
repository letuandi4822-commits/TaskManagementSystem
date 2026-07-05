using TaskManagementSystem.Data;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Seed
{
    public class DatabaseSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Users.Any())
            {
                return;
            }
            var admin = new User
            {
                Username = "admin",
                Email = "admin@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                Role = "Admin"
            };

            var user = new User
            {
                Username = "john",
                Email = "john@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                Role = "User"
            };

            context.Users.AddRange(admin, user);
            context.SaveChanges();
            var project1 = new Project
            {
                Name = "Website Redesign",
                Description = "Redesign company website using ASP.NET Core MVC.",
                UserId = admin.Id
            };

            var project2 = new Project
            {
                Name = "Inventory Management System",
                Description = "Build inventory management features.",
                UserId = admin.Id
            };

            context.Projects.AddRange(project1, project2);
            context.SaveChanges();

            var task1 = new TaskItem
            {
                Title = "Design Login UI",
                Description = "Create responsive login page.",
                Status = "Done",
                StartDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(5),
                ProjectId = project1.Id,
                AssignedUserId = user.Id,
            };

            var task2 = new TaskItem
            {
                Title = "Build Dashboard",
                Description = "Implement dashboard with charts.",
                Status = "In Progress",
                StartDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                ProjectId = project1.Id,
                AssignedUserId = user.Id
            };
            context.Tasks.AddRange(task1, task2);
            context.SaveChanges();

            context.Notifications.AddRange(

            new Notification
            {
                UserId = user.Id,
                TaskId = task1.Id,
                Message = $"You have been assigned task '{task1.Title}'."
            },
            new Notification
            {
                UserId = user.Id,
                TaskId = task2.Id,
                Message = $"You have been assigned task '{task2.Title}'."
            }
                );
            context.SaveChanges();
            context.ActivityLogs.AddRange(

            new ActivityLog
            {
                UserId = admin.Id,
                Module = "Project",
                Action = "Created project Website Redesign"
            },

            new ActivityLog
            {
                UserId = admin.Id,
                Module = "Project",
                Action = "Created project Inventory Management System"
            }
            );
            context.SaveChanges();
        }
        
    }
}
