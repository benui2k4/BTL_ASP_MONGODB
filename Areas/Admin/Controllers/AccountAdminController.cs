using ASP_MongoDB.Models;
using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASP_MongoDB.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]  // Chỉ cho phép Admin truy cập
    public class AccountAdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<MongoIdentityRole<Guid>> _roleManager;

        public AccountAdminController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<MongoIdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // GET: /AccountAdmin/LoginAdmin
        [HttpGet]
        [AllowAnonymous]
        public IActionResult LoginAdmin()
        {
            return View();
        }

        // POST: /AccountAdmin/LoginAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAdmin(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {

                    var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
                    if (isPasswordValid)
                    {
                        var principal = await _signInManager.CreateUserPrincipalAsync(user);

                        // Kiểm tra Claims và Roles của user
                        var roles = await _userManager.GetRolesAsync(user);  
                        if (roles.Contains("Admin"))
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);

                            TempData["success"] = "Đăng nhập thành công !";
                            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                        }
                        else
                        {
                            ModelState.AddModelError("", "Bạn không có quyền truy cập vào khu vực này.");
                        }
                    }
                    else
                    {
                        TempData["error"] = "Đăng nhập thất bại.Tài khoản hoặc mật khẩu không chính xác!";
                        

                    }
                }
                else
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tại. Vui lòng kiểm tra lại!");
                }

            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        // GET: /AccountAdmin/RegisterAdmin
        public IActionResult RegisterAdmin()
        {
            return View();
        }

        // POST: /AccountAdmin/RegisterAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAdmin(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    EmailConfirmed = true,
                    FullName = model.FullName,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    PhoneNumberConfirmed = true,
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Kiểm tra nếu role "Admin" chưa tồn tại, tạo mới
                    if (!await _roleManager.RoleExistsAsync("Admin"))
                    {
                        var role = new MongoIdentityRole<Guid> { Name = "Admin" };  // Tạo role với tên "Admin"
                        await _roleManager.CreateAsync(role);  // Tạo role trong MongoDB
                    }

                    // Thêm người dùng vào role "Admin" dựa trên tên role, không phải UUID
                    var addRoleResult = await _userManager.AddToRoleAsync(user, "Admin");  // Sử dụng tên role

                    if (addRoleResult.Succeeded)
                    {
                        var claimResult = await _userManager.AddClaimAsync(user, new Claim("Role", "Admin"));

                        if (claimResult.Succeeded)
                        {
                            TempData["success"] = "Tạo tài khoản thành công!";
                            return RedirectToAction("LoginAdmin");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Lỗi khi gán claim cho người dùng!");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Lỗi khi gán vai trò Admin cho người dùng!");
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }


        // POST: /AccountAdmin/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]  // Đảm bảo rằng có attribute này
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["success"] = "Bạn đã đăng xuất thành công!";
            return RedirectToAction("LoginAdmin", "AccountAdmin");
        }


    }
}
