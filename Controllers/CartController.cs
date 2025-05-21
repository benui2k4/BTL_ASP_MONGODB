using ASP_MongoDB.Data;
using ASP_MongoDB.Extension;
using ASP_MongoDB.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ASP_MongoDB.Controllers
{
    public class CartController : Controller
    {
        private readonly IMongoCollection<Product> _productsCollection;

        public CartController(MongoDBContext context)
        {
            _productsCollection = context.Database.GetCollection<Product>("Product");
        }

        // Hiển thị giỏ hàng
        [HttpGet]
        public IActionResult Index()
        {
            var cartItems = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var cartVM = new CartViewModel()
            {
                CartItems = cartItems
            };
            return View(cartVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(string productId)
        {
            var product = await _productsCollection.Find(p => p.ProductId == productId).FirstOrDefaultAsync();
            if (product == null)
                return NotFound();

            var cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);
            if (cartItem == null)
            {
                cart.Add(new CartItem(product));
            }
            else
            {
                cartItem.Quantity += 1;
            }

            HttpContext.Session.SetJson("Cart", cart);
            TempData["success"] = "Thêm sản phẩm vào giỏ hàng thành công!";

            return RedirectToAction("Index");  // Chuyển về trang giỏ hàng sau khi thêm thành công
        }



        
        public IActionResult Increase(string productId)
        {
            var cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");
            if (cart == null)
                return RedirectToAction("Index");

            var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity++;
                HttpContext.Session.SetJson("Cart", cart);
            }
            TempData["success"] = "Cập nhật số lượng giỏ hàng thành công!";
            return RedirectToAction("Index");
        }

        // Giảm số lượng sản phẩm
        public IActionResult Decrease(string productId)
        {
            var cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");
            if (cart == null)
                return RedirectToAction("Index");

            var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity--;
                if (cartItem.Quantity <= 0)
                    cart.Remove(cartItem);

                if (cart.Count == 0)
                    HttpContext.Session.Remove("Cart");
                else
                    HttpContext.Session.SetJson("Cart", cart);
            }
            TempData["success"] = "Cập nhật số lượng giỏ hàng thành công!";
            return RedirectToAction("Index");
        }

        // Xóa sản phẩm khỏi giỏ hàng
        public IActionResult Remove(string productId)
        {
            var cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");
            if (cart == null)
                return RedirectToAction("Index");

            cart.RemoveAll(c => c.ProductId == productId);
            if (cart.Count == 0)
                HttpContext.Session.Remove("Cart");
            else
                HttpContext.Session.SetJson("Cart", cart);

            TempData["success"] = "Xóa sản phẩm khỏi giỏ hàng thành công!";
            return RedirectToAction("Index");
        }

        // Xóa toàn bộ giỏ hàng
        public IActionResult Clear()
        {
            HttpContext.Session.Remove("Cart");
            TempData["success"] = "Xóa tất cả giỏ hàng thành công!";
            return RedirectToAction("Index");
        }
    }
}
