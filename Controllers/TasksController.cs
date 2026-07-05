using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Security.Claims;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services;
using TaskManagementSystem.ViewModel;
namespace TaskManagementSystem.Controllers
{
    [Authorize(Roles ="Admin")]
    public class TasksController:Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;
        public TasksController(ApplicationDbContext context , EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
 
        public IActionResult Index(int projectID)
        {
            var tasks = _context.Tasks.Where(x => x.ProjectId == projectID).ToList();
            ViewBag.ProjectId = projectID;
            return View(tasks);
        }

        [HttpGet]
  
        public IActionResult Create(int projectId)
        {
            ViewBag.ProjectId = projectId;
            ViewBag.Users = _context.Users.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTaskViewModel model,int projectId)
        {
            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var project = _context.Projects
                .FirstOrDefault( p => p.Id == projectId);

            if (project == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ProjectId = projectId;
                ViewBag.Users = _context.Users.ToList();
                return View(model);
            }
            if (model.DueDate <= DateTime.Now)
            {
                ModelState.AddModelError(" ", "ngày kết thúc phải lớn hơn ngày bắt đầu");
                ViewBag.ProjectId = projectId;
                ViewBag.Users = _context.Users.ToList();
                return View(model);
            }

            var task = new TaskItem
            {
                Title = model.Title,
                Description = model.Description,
                Status = model.Status,
                StartDate = DateTime.Now,
                DueDate = model.DueDate,
                ProjectId = projectId,
                AssignedUserId = model.AssignedUserId

            };

            var log = new ActivityLog
            {
                UserId = adminId,
                Module = "Task",
                Action = $"Create Task:{task.Title}"
            };
            _context.ActivityLogs.Add(log);
            _context.Tasks.Add(task);
          
            await _context.SaveChangesAsync();
            var notification = new Notification
            {
                UserId = task.AssignedUserId,
                Message = $"You have been assigned task '{task.Title}'.",
                TaskId = task.Id
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            var assignedUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == task.AssignedUserId);
            if (assignedUser != null)
            {
                var subject = "You have been assigned a new task";
                var body = _emailService.LoadTemplate("TaskAssigned.html");
                body = body.Replace("{{Username}}", assignedUser.Username);
                body = body.Replace("{{Title}}", task.Title);
                body = body.Replace("{{Description}}", task.Description);
                body = body.Replace("{{Status}}", task.Status);
                body = body.Replace("{{DueDate}}",
                    task.DueDate.ToString("dd/MM/yyyy"));
                await _emailService.SendEmail(assignedUser.Email, subject, body);
            }

            return RedirectToAction( "Index",new { projectId });
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
         
            var task = _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            var model = new EditTasKViewModel
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(EditTasKViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var task = _context.Tasks
                .Include(p => p.Project)
                .FirstOrDefault(p => p.Id == model.Id);

            if (task == null)
            {
                return NotFound();
            }

            if(model.DueDate <= model.StartDate)
            {
                ModelState.AddModelError(" " ,"ngày kết thúc phải lớn hơn ngày bắt đầu");
                
                return View(model);
            }

            task.Title = model.Title;
            task.Description = model.Description;
            task.Status = model.Status;
            task.DueDate = model.DueDate;


            var log = new ActivityLog
            {
                UserId = adminId,
                Module = "Task",
                Action = $"Update Task:{task.Title}"
            };
            _context.ActivityLogs.Add(log);
            _context.SaveChanges();

            return RedirectToAction(
                "Index",
                new { projectId = task.ProjectId });
        }

        [HttpPost]
        public IActionResult Delete(int id )
        {
            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!
        .Value);

            var task = _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefault(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            int projectId = task.ProjectId;

            var log = new ActivityLog
            {
                UserId = adminId,
                Module = "Task",
                Action = $"Delete Task:{task.Title}"
            };
            _context.ActivityLogs.Add(log);
            _context.Tasks.Remove(task);

            _context.SaveChanges();

            return RedirectToAction("Index", new { projectId });
        }
    }
}
