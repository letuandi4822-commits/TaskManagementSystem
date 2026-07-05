using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers
{
    [Authorize(Roles ="User")]
    public class NotificationsController:Controller
    {
        private readonly ApplicationDbContext _context;
        public NotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var currentUserId =int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var notifications =
                _context.Notifications
                .Where(x => x.UserId == currentUserId)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            foreach (var item in notifications)
            {
                item.IsRead = true;
            }

            _context.SaveChanges();

            return View(notifications);
        }
    }
}
