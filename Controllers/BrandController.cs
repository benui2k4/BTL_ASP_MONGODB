using ASP_MongoDB.Data;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ASP_MongoDB.Controllers
{
    public class BrandController : Controller
    {
        private readonly MongoDBContext _context;


        public BrandController (MongoDBContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(string id = "")
        {
            // Kiểm tra brand tồn tại
            var brand = await _context.Brand.Find(b => b.BrandId == id).FirstOrDefaultAsync();

            if (brand == null)
                return RedirectToAction("Index", "Home");

            // Lấy danh sách sản phẩm theo brand
            var products = await _context.Product.Find(p => p.Brand == id).ToListAsync();

            // Lấy thông tin category để hiển thị tên
            var categories = await _context.Category.Find(_ => true).ToListAsync();

            foreach (var product in products)
            {
                var category = categories.FirstOrDefault(c => c.CategoryId == product.Category);
                product.CategoryName = category?.CategoryName ?? "";
                product.BrandName = brand.BrandName;
            }

            return View(products);
        }
    }
}
