using ASP_MongoDB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASP_MongoDB.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AccountManagerController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountManagerController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Danh sách tất cả tài khoản
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList(); // lấy toàn bộ user trong MongoDB
            var userRoles = new Dictionary<string, IList<string>>();

            foreach (var user in users)
            {
                userRoles[user.Id.ToString()] = await _userManager.GetRolesAsync(user); // Chuyển Guid sang string
            }

            ViewBag.UserRoles = userRoles;

            return View(users);
        }

        // Chi tiết tài khoản
        public async Task<IActionResult> Details(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound();

            ViewBag.Roles = await _userManager.GetRolesAsync(user);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);

            TempData["success"] = "Cập nhật trạng thái tài khoản thành công!";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> ChangePassword(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                TempData["error"] = "Người dùng không tồn tại!";
                return RedirectToAction("Index");
            }

            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(Guid id, string oldPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                TempData["error"] = "Mật khẩu mới và xác nhận mật khẩu không khớp!";
                return RedirectToAction("ChangePassword", new { id });
            }

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                TempData["error"] = "Người dùng không tồn tại!";
                return RedirectToAction("Index");
            }

            // Kiểm tra mật khẩu cũ
            var checkPassword = await _userManager.CheckPasswordAsync(user, oldPassword);
            if (!checkPassword)
            {
                TempData["error"] = "Mật khẩu cũ không chính xác!";
                return RedirectToAction("ChangePassword", new { id });
            }

            // Đặt mật khẩu mới
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
            if (result.Succeeded)
            {
                TempData["success"] = "Mật khẩu đã được thay đổi thành công!";
            }
            else
            {
                TempData["error"] = string.Join("<br>", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction("Details", new { id });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            if (user.IsActive || user.Id.ToString() == User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                TempData["error"] = "Chỉ có thể xóa tài khoản đã bị khóa và không phải tài khoản đang đăng nhập.";
                return RedirectToAction("Index");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["success"] = "Xóa tài khoản thành công!";
            }
            else
            {
                TempData["error"] = "Lỗi khi xóa tài khoản.";
            }

            return RedirectToAction("Index");
        }
    }
}
