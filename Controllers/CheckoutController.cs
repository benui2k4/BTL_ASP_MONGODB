using ASP_MongoDB.Data;
using ASP_MongoDB.Extension;
using ASP_MongoDB.Models;
using ASP_MongoDB.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;
using MongoDB.Bson;

namespace ASP_MongoDB.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly MongoDBContext _context;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckoutController(
            MongoDBContext context,
            IEmailSender emailSender,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = HttpContext.Session.GetJson<List<CartItem>>("Cart");
            if (cartItems == null || cartItems.Count == 0)
            {
                TempData["error"] = "Giỏ hàng đang trống!";
                return RedirectToAction("Index", "Cart");
            }

            var order = new Order
            {
                OrderId = Guid.NewGuid().ToString(),
                FullName = user.FullName,
                EmailAddress = user.Email,
                Phone = user.PhoneNumber,
                Address = user.Address,
                CreateAt = DateTime.Now,
                Status = OrderStatus.ChoDuyet,
                Price = cartItems.Sum(c => c.Price * c.Quantity),
                OrderItems = cartItems.Select(c => new OrderItem
                {
                    ProductId = ObjectId.Parse(c.ProductId),
                    ProductName = c.ProductName,
                    Price = c.Price,
                    Quantity = c.Quantity,
                    Image = c.Image
                }).ToList()
            };

            await _context.Order.InsertOneAsync(order);

            var sb = new StringBuilder();
            sb.AppendLine("<h2>Thông tin người đặt hàng</h2>");
            sb.AppendLine($"<p><strong>Họ tên:</strong> {order.FullName}</p>");
            sb.AppendLine($"<p><strong>Email:</strong> {order.EmailAddress}</p>");
            sb.AppendLine($"<p><strong>Điện thoại:</strong> {order.Phone}</p>");
            sb.AppendLine($"<p><strong>Địa chỉ:</strong> {order.Address}</p>");
            sb.AppendLine($"<p><strong>Ngày đặt:</strong> {order.CreateAt:dd/MM/yyyy HH:mm}</p>");
            sb.AppendLine("<hr/>");
            sb.AppendLine("<h3>Chi tiết đơn hàng</h3>");
            sb.AppendLine("<table border='1' cellpadding='10' cellspacing='0' style='border-collapse:collapse; text-align:center;'>");
            sb.AppendLine("<tr><th>Ảnh</th><th>Tên sản phẩm</th><th>Số lượng</th><th>Đơn giá</th><th>Tổng tiền</th></tr>");

            foreach (var item in order.OrderItems)
            {
                string imageUrl = item.Image;
                if (!imageUrl.StartsWith("http"))
                {
                    imageUrl = $"{Request.Scheme}://{Request.Host}/{imageUrl.TrimStart('/')}";
                }

                sb.AppendLine("<tr>");
                sb.AppendLine($"<td><img src='{imageUrl}' alt='{item.ProductName}' width='80' /></td>");
                sb.AppendLine($"<td>{item.ProductName}</td>");
                sb.AppendLine($"<td>{item.Quantity}</td>");
                sb.AppendLine($"<td>{item.Price.ToString("N0")} VNĐ</td>");
                sb.AppendLine($"<td>{(item.Price * item.Quantity).ToString("N0")} VNĐ</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");
            sb.AppendLine($"<p><strong>Tổng tiền:</strong> {order.Price.ToString("N0")} VNĐ</p>");

            await _emailSender.SendEmailAsync(user.Email, "Xác nhận đơn hàng từ Shopper Online", sb.ToString());

            HttpContext.Session.Remove("Cart");

            TempData["success"] = "Đơn hàng đã được tạo thành công! Vui lòng kiểm tra gmail để xem chi tiết.";

            return RedirectToAction("Index", "Cart");
        }
    }
}
