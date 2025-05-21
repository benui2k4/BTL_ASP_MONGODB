using ASP_MongoDB.Data;
using ASP_MongoDB.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace ASP_MongoDB.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly MongoDBContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductsController(MongoDBContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index(string searchTerm, int pg = 1)
        {
            const int pageSize = 5;

            var filter = Builders<Product>.Filter.Empty;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                filter = Builders<Product>.Filter.Regex(p => p.ProductName, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"));
            }

            var totalItems = await _context.Product.CountDocumentsAsync(filter);
            var pager = new Paginate((int)totalItems, pg, pageSize);

            var products = await _context.Product
                .Find(filter)
                .SortByDescending(p => p.ProductId)
                .Skip((pg - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            // Lấy toàn bộ danh mục và thương hiệu
            var categories = await _context.Category.Find(_ => true).ToListAsync();
            var brands = await _context.Brand.Find(_ => true).ToListAsync();

            // Gán tên cho từng sản phẩm
            foreach (var product in products)
            {
                var category = categories.FirstOrDefault(c => c.CategoryId == product.Category);
                var brand = brands.FirstOrDefault(b => b.BrandId == product.Brand);

                product.CategoryName = category?.CategoryName ?? "";
                product.BrandName = brand?.BrandName ?? "";
            }

            ViewBag.Pager = pager;
            ViewBag.SearchTerm = searchTerm;

            return View(products);
        }

        // GET: Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Category.Find(_ => true).ToListAsync();
            ViewBag.Brands = await _context.Brand.Find(_ => true).ToListAsync();
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile imageFile)
        {
            if (product.Category == null || product.Brand == null)
            {
                ModelState.AddModelError("Category", "Danh mục và thương hiệu là bắt buộc!");
            }

            // Kiểm tra ModelState nếu có lỗi
            if (ModelState.IsValid)
            {
                // Xử lý file ảnh nếu có
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var folderPath = Path.Combine(_env.WebRootPath, "images", "products");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    var filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    product.Image = "/images/products/" + fileName;
                }

                await _context.Product.InsertOneAsync(product);
                TempData["success"] = "Thêm mới sản phẩm thành công!";
                return RedirectToAction(nameof(Index));  
            }
            ViewBag.Categories = await _context.Category.Find(_ => true).ToListAsync();
            ViewBag.Brands = await _context.Brand.Find(_ => true).ToListAsync();

            return View(product);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(string id)
        {
            var product = await _context.Product.Find(p => p.ProductId == id).FirstOrDefaultAsync();
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await _context.Category.Find(_ => true).ToListAsync();
            ViewBag.Brands = await _context.Brand.Find(_ => true).ToListAsync();
            return View(product);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Product product, IFormFile imageFile)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var folderPath = Path.Combine(_env.WebRootPath, "images", "products");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    var filePath = Path.Combine(folderPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    product.Image = "/images/products/" + fileName;
                }

                var update = Builders<Product>.Update
                    .Set(p => p.ProductName, product.ProductName)
                    .Set(p => p.Price, product.Price)
                    .Set(p => p.Quantity, product.Quantity)
                    .Set(p => p.Description, product.Description)
                    .Set(p => p.Category, product.Category)
                    .Set(p => p.Brand, product.Brand)
                    .Set(p => p.Image, product.Image);

                await _context.Product.UpdateOneAsync(p => p.ProductId == id, update);
                TempData["success"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await _context.Category.Find(_ => true).ToListAsync();
            ViewBag.Brands = await _context.Brand.Find(_ => true).ToListAsync();
            return View(product);
        }

        public async Task<IActionResult> Details(string id)
        {
            var product = await _context.Product.Find(p => p.ProductId == id).FirstOrDefaultAsync();
            if (product == null)
            {
                return NotFound();
            }

            var category = await _context.Category.Find(c => c.CategoryId == product.Category).FirstOrDefaultAsync();
            var brand = await _context.Brand.Find(b => b.BrandId == product.Brand).FirstOrDefaultAsync();

            product.CategoryName = category?.CategoryName ?? "";
            product.BrandName = brand?.BrandName ?? "";

            return View(product);
        }
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["error"] = "Không tìm thấy mã sản phẩm!";
                return RedirectToAction("Index");
            }

            var result = await _context.Product.DeleteOneAsync(p => p.ProductId == id);

            if (result.DeletedCount == 0)
            {
                TempData["error"] = "Không tìm thấy sản phẩm để xóa.";
            }
            else
            {
                TempData["success"] = "Xóa sản phẩm thành công!";
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
