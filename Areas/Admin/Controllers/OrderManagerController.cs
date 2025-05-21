using ASP_MongoDB.Data;
using ASP_MongoDB.Models;
using ASP_MongoDB.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace ASP_MongoDB.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderManagerController : Controller
    {
        private readonly MongoDBContext _context;

        public OrderManagerController(MongoDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTerm, int pg = 1)
        {
            const int pageSize = 10;

            var filter = Builders<Order>.Filter.Empty;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                filter = Builders<Order>.Filter.Regex("FullName", new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"));
            }

            var ordersCollection = _context.Order.Find(filter);
            var totalOrders = await ordersCollection.CountDocumentsAsync();

            if (pg < 1) pg = 1;
            int skip = (pg - 1) * pageSize;

            var orders = await ordersCollection
                .SortByDescending(o => o.CreateAt)
                .Skip(skip)
                .Limit(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = pg;
            ViewBag.TotalPages = (int)System.Math.Ceiling((double)totalOrders / pageSize);
            ViewBag.SearchTerm = searchTerm;

            return View(orders);
        }


        public async Task<IActionResult> Details(string id)
        {
            var order = await _context.Order.Find(o => o.OrderId == id).FirstOrDefaultAsync();
            if (order == null) return NotFound();

            foreach (var item in order.OrderItems)
            {
                var product = await _context.Product.Find(p => p.ProductId == item.ProductId.ToString()).FirstOrDefaultAsync();
                if (product != null)
                {
                    item.ProductName = product.ProductName;
                    item.Image = product.Image;

                    var category = await _context.Category.Find(c => c.CategoryId == product.Category).FirstOrDefaultAsync();
                    if (category != null)
                    {
                        item.CategoryName = category.CategoryName;
                    }

                    var brand = await _context.Brand.Find(b => b.BrandId == product.Brand).FirstOrDefaultAsync();
                    if (brand != null)
                    {
                        item.BrandName = brand.BrandName;
                    }
                }
            }

            return View(order);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateStatus(string id)
        {
            var order = await _context.Order.Find(o => o.OrderId == id).FirstOrDefaultAsync();

            if (order == null || order.Status == OrderStatus.DaDuyet)
            {
                TempData["error"] = "Không thể cập nhật trạng thái";
            }

            var update = Builders<Order>.Update.Set(o => o.Status, OrderStatus.DaDuyet);
            await _context.Order.UpdateOneAsync(o => o.OrderId == id, update);

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var result = await _context.Order.DeleteOneAsync(o => o.OrderId == id);

            if (result.DeletedCount == 0)
            {
                return NotFound();
            }

            return RedirectToAction("Index");
        }

    }
}
