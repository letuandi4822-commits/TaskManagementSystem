using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Data;
using System.Security.Claims;
using TaskManagementSystem.ViewModel;
using TaskManagementSystem.Models;
namespace TaskManagementSystem.Controllers
{

    public class DashboardController: Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }
            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var assignedTasks = _context.Tasks.Count(p=>p.AssignedUserId == currentUserId);
            var CompletedTasks = _context.Tasks.Count(p => p.AssignedUserId == currentUserId && p.Status == "Done");
            var PendingTasks = _context.Tasks.Count(p => p.AssignedUserId == currentUserId && p.Status != "Done");
            var OverdueTasks = _context.Tasks.Count(p => p.AssignedUserId == currentUserId && p.DueDate < DateTime.Now && p.Status != "Done");

            var model = new DashboardViewModel
            {
                AssignedTasks = assignedTasks,
                CompletedTasks = CompletedTasks,
                PendingTasks = PendingTasks,
                OverdueTasks = OverdueTasks
            };
            return View(model);
        }
    }
    

}
