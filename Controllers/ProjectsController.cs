using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;
using TaskManagementSystem.ViewModel;

namespace TaskManagementSystem.Controllers
{
    [Authorize (Roles ="Admin")]
    public class ProjectsController:Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateProjectViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var userIdClaim =User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            int adminId = int.Parse(userIdClaim.Value);
            var project = new Project
            {
                Name = model.Name,
                Description = model.Description,
                UserId = adminId
            };

            
            var log = new ActivityLog
            {
                UserId = adminId,
                Module = "Project",
                Action = $"Create Projects:{project.Name}"
            };

            _context.ActivityLogs.Add(log);

            _context.Projects.Add(project);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        [HttpGet]
        public IActionResult Index()
        {
            

            var projects = _context.Projects.ToList();

            return View(projects);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var project = _context.Projects
                .FirstOrDefault(p => p.Id == id);

            if(project == null)
            {
                return NotFound();

            }
            var model = new EditProjectViewModel
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(EditProjectViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            int adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var project = _context.Projects
                .FirstOrDefault(p => p.Id == model.Id );

            if (project == null)
            {
                return NotFound();
            }

            project.Name = model.Name;
            project.Description = model.Description;
            var log = new ActivityLog
            {
                UserId = adminId,
                Module = "Project",
                Action = $"Update Task:{project.Name}"
            };
            _context.ActivityLogs.Add(log);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var project = _context.Projects
                .FirstOrDefault(p => p.Id == id );


            if (project == null)
            {
                return NotFound();
            }
            var log = new ActivityLog
            {
                UserId = userId,
                Module = "Project",
                Action = $"Delete Project:{project.Name}"
            };
            _context.ActivityLogs.Add(log);

            _context.Projects.Remove(project);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}
