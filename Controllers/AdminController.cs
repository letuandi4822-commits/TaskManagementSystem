using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services;
using TaskManagementSystem.ViewModel;

namespace TaskManagementSystem.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController :Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public AdminController(ApplicationDbContext context , EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var totalUsers = _context.Users.Count();
            var totalProjects = _context.Projects.Count();
            var totalTasks = _context.Tasks.Count();
            var completedTasks = _context.Tasks.Where(p => p.Status == "Done").Count();
            var pendingTasks = _context.Tasks.Where(p =>  p.Status != "Done").Count();
            var overdueTasks = _context.Tasks.Where(p => p.DueDate < DateTime.Now && p.Status != "Done").Count();
            var model = new AdminDashBroadViewModel
            {
                TotalProjects = totalProjects,
                TotalTasks = totalTasks,
                TotalUsers = totalUsers,
                CompletedTasks = completedTasks,
                PendingTasks = pendingTasks,
                OverdueTasks = overdueTasks
            };
            return View(model);
        }
        
        public IActionResult Users()
        {
            var users = _context.Users.ToList();
            return View(users);
        }
        
        [HttpPost]        
        public IActionResult ChangeRole(int id )
        {
            var user = _context.Users.FirstOrDefault(p => p.Id == id);
            if(user == null)
            {
                return NotFound();
            }    

            if(user.Role == "Admin")
            {
                user.Role = "User";
            }
            else
            {
                user.Role = "Admin";
            }
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var log = new ActivityLog
            {
                UserId = currentUserId,
                Module = "User",
                Action = $"Changed role of {user.Username} to {user.Role}"

            };
            _context.ActivityLogs.Add(log); 
            _context.SaveChanges();
     
            return RedirectToAction("Users");
        } 
        
        public IActionResult Projects()
        {
            var project = _context.Projects.Include(p=>p.User).ToList();
            return View(project);
        }

        public IActionResult Tasks()
        {
            var task = _context.Tasks.Include(p => p.Project).ThenInclude(p=>p.User).ToList();
            return View(task);
        }

        public IActionResult Logs (string search,string module,DateTime?fromDate,DateTime? toDate)
        {
            var log = _context.ActivityLogs.Include(p => p.User).OrderByDescending(x => x.CreatedAt).AsQueryable();
            if (!string.IsNullOrEmpty(module))
            {
                log = log.Where(p => p.Module == module);
            }
            if (!string.IsNullOrEmpty(search))
            {
                var keyword = search.ToLower();
                log = log.Where(p => p.Action.ToLower().Contains(keyword) || p.User.Username.ToLower().Contains(keyword));  
            }
            if (fromDate.HasValue)
            {

                log = log.Where(p => p.CreatedAt >= fromDate);
            }
            if (toDate.HasValue)
            {
                log = log.Where(p => p.CreatedAt <= toDate);
            }             

            return View(log.ToList());
        }

      
        
    }
}
