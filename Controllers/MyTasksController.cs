using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Security.Claims;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;
using TaskManagementSystem.ViewModel;

namespace TaskManagementSystem.Controllers
{
    [Authorize(Roles ="User")]
    public class MyTasksController:Controller
    {
        private readonly ApplicationDbContext _context;
        public MyTasksController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var tasks = _context.Tasks.Include(p=> p.Project).Where(p=>p.AssignedUserId == currentUserId).ToList();
            return View(tasks);
        }
        public IActionResult UpdateStatus(int id)
        {
            var currenUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var task = _context.Tasks
                .Include(p => p.Project)
                .FirstOrDefault(p => p.Id == id && p.AssignedUserId == currenUserId );
            if (task == null)
            {
                return NotFound();
            }

            var model = new UpdateTaskStatusViewModel
            {
                Id = task.Id,
                Status = task.Status
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateStatus(UpdateTaskStatusViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var task = _context.Tasks
                .Include(p => p.Project)
                .FirstOrDefault(p => p.Id == model.Id && p.AssignedUserId == currentUserId );

            if (task == null)
            {
                return NotFound();
            }

            task.Status = model.Status;
        

            var log = new ActivityLog
            {
                UserId = currentUserId,
                Module = "Task",
                Action = $"Updated status of '{task.Title}' to '{task.Status}'"
            };
            _context.ActivityLogs.Add(log);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult UploadFile(int taskId)
        {
            return View(new UploadTaskFileViewModel
            {
                TaskId = taskId
            });
        }
        [HttpPost]
        public  async Task<IActionResult> UploadFile(UploadTaskFileViewModel model)
        {
            if(model.File == null || model.File.Length == 0)
            {
                ModelState.AddModelError("", "Please select a file.");
                return View(model);
            }

            var filename = Guid.NewGuid() + Path.GetExtension(model.File.FileName);
            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Tasks");
            var filePath = Path.Combine(uploadFolder, filename);
            using (var stream = new FileStream(filePath , FileMode.Create)) 
            {
                await model.File.CopyToAsync(stream);
            }

            var attchament = new TaskAttachment
            {
                TaskId = model.TaskId,
                FileName = model.File.FileName,
                FilePath = "/Uploads/Tasks/" + filename

            };
            _context.TaskAttachments.Add(attchament);
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Index");

        }
        public IActionResult File(int taskId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var task = _context.Tasks.FirstOrDefault(p=>p.Id == taskId && p.AssignedUserId == currentUserId);

            if(task == null)
            {
                return NotFound();
            }

            var file = _context.TaskAttachments.Where(p=> p.TaskId == taskId).ToList();

            ViewBag.TaskId = taskId;

            return View(file);
        }

        public IActionResult Download(int Id)
        {
            var file = _context.TaskAttachments.FirstOrDefault(p => p.Id == Id);
            if (file == null)
            {
                return NotFound();
                
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (!System.IO.File.Exists(path))
            {
                //return NotFound();
                return Content(path);
            }

            return PhysicalFile(path, "application/octet-stream", file.FileName);
      
        }
        [HttpPost]
        public IActionResult DeleteFile(int Id)
        {
            var currnentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var file = _context.TaskAttachments.Include(p=>p.Task).FirstOrDefault(p=>p.Id == Id && p.Task.AssignedUserId == currnentId);

            if( file == null)   
            { 
                return NotFound(); 
            }

            var taskId = file.TaskId;

            var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot",
                file.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            _context.TaskAttachments.Remove(file);
            _context.SaveChanges();

            return RedirectToAction("File", new { taskId });
        }
    }
}
