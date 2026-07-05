using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services;
using TaskManagementSystem.ViewModel;


namespace TaskManagementSystem.Controllers;


public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly PasswordService _passwordService;

    public AccountController(
        ApplicationDbContext context,
        PasswordService passwordService)
    {
        _context = context;
        _passwordService = passwordService;
    }
    
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var existingUser = _context.Users.FirstOrDefault(x => x.Username == model.Username);
        if (existingUser != null)
        {
            ModelState.AddModelError("", "Username đã tồn tại");
            return View(model);
        }


        var existingEmail =_context.Users.FirstOrDefault(x => x.Email == model.Email);
        if (existingEmail != null)
        {
            ModelState.AddModelError(
                "",
                "Email đã tồn tại");

            return View(model);
        }


        var user = new User
        {
            Username = model.Username,
            Email = model.Email,
            PasswordHash = _passwordService.Hash(model.Password)

        };

        _context.Users.Add(user);

        _context.SaveChanges();

        return RedirectToAction("Login");
    }
 
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = _context.Users
            .FirstOrDefault(
                x => x.Email == model.Email);

        if (user == null)
        {
            ModelState.AddModelError(
                "",
                "Email không tồn tại");

            return View(model);
        }

        bool isValid =
            _passwordService.Verify(
                model.Password,
                user.PasswordHash);

        if (!isValid)
        {
            ModelState.AddModelError(
                "",
                "Mật khẩu không đúng");

            return View(model);
        }
       
        var claims = new List<Claim>
        {

            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            new Claim(
                ClaimTypes.Name,
                user.Username),

            new Claim(
                ClaimTypes.Role,
                user.Role)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);


        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        return RedirectToAction("Index", "Home");
    }
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction(
        "Login",
        "Account");
    }


}
