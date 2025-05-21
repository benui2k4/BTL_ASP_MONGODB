using ASP_MongoDB.Models;
using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASP_MongoDB.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<MongoIdentityRole<Guid>> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 RoleManager<MongoIdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    FullName = model.FullName,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Kiểm tra và tạo role "User" nếu chưa tồn tại
                    if (!await _roleManager.RoleExistsAsync("User"))
                    {
                        var role = new MongoIdentityRole<Guid> { Name = "User" };
                        await _roleManager.CreateAsync(role);
                    }

                    // Gán role "User" cho tài khoản mới
                    await _userManager.AddToRoleAsync(user, "User");
                    await _userManager.AddClaimAsync(user, new Claim("Role", "User"));

                    TempData["success"] = "Đăng ký tài khoản thành công!";
                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    
                    if (!user.IsActive)
                    {
                        TempData["error"] = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ với bộ phận quản trị hệ thống!";
                        return View(model);
                    }

                    // Kiểm tra mật khẩu
                    if (await _userManager.CheckPasswordAsync(user, model.Password))
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        TempData["success"] = "Đăng nhập thành công!";
                        return RedirectToAction("Index", "Home");
                    }
                }

                // Sai email hoặc mật khẩu
                TempData["error"] = "Email hoặc mật khẩu không chính xác. Vui lòng kiểm tra lại!";
            }

            return View(model);
        }



        // GET: /Account/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["success"] = "Đăng xuất thành công!";
            return RedirectToAction("Login");
        }
    }
}
