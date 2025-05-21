using ASP_MongoDB.Data;
using ASP_MongoDB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace ASP_MongoDB.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BrandsController : Controller
    {
        private readonly MongoDBContext _context;

        public BrandsController(MongoDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTerm, int pg = 1)
        {
            const int pageSize = 5;

            var filter = Builders<Brands>.Filter.Empty;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                filter = Builders<Brands>.Filter.Regex(b => b.BrandName, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"));
            }

            var totalItems = await _context.Brand.CountDocumentsAsync(filter);
            var pager = new Paginate((int)totalItems, pg, pageSize);

            var brands = await _context.Brand
                .Find(filter)
                .SortByDescending(b => b.BrandId)
                .Skip((pg - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            ViewBag.Pager = pager;
            ViewBag.SearchTerm = searchTerm;

            return View(brands);
        }

        // GET: Admin/Brands/Create 
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Brands/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Brands brand)
        {
            if (ModelState.IsValid)
            {
                var existingBrand = await _context.Brand
                    .Find(b => b.BrandName.ToLower() == brand.BrandName.ToLower())
                    .FirstOrDefaultAsync();

                if (existingBrand != null)
                {
                    ModelState.AddModelError("BrandName", "Tên thương hiệu đã tồn tại. Vui lòng kiểm tra lại!");
                    return View(brand);
                }

                await _context.Brand.InsertOneAsync(brand);
                TempData["success"] = "Thêm mới thương hiệu thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(brand);
        }

        // GET: Admin/Brands/Edit/{id}
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["error"] = "Lỗi: Không tìm thấy mã thương hiệu.";
                return RedirectToAction(nameof(Index));
            }

            var brand = await _context.Brand.Find(b => b.BrandId == id).FirstOrDefaultAsync();
            if (brand == null)
            {
                TempData["error"] = "Lỗi: Không tìm thấy thương hiệu.";
                return RedirectToAction(nameof(Index));
            }

            return View(brand);
        }

        // POST: Admin/Brands/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Brands brand)
        {
            if (id != brand.BrandId)
            {
                TempData["error"] = "Lỗi: ID thương hiệu không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                var existingBrand = await _context.Brand
                    .Find(b => b.BrandName.ToLower() == brand.BrandName.ToLower() && b.BrandId != id)
                    .FirstOrDefaultAsync();

                if (existingBrand != null)
                {
                    ModelState.AddModelError("BrandName", "Tên thương hiệu đã tồn tại. Vui lòng kiểm tra lại!");
                    return View(brand);
                }

                var filter = Builders<Brands>.Filter.Eq(b => b.BrandId, id);
                var update = Builders<Brands>.Update.Set(b => b.BrandName, brand.BrandName);

                await _context.Brand.UpdateOneAsync(filter, update);

                TempData["success"] = "Cập nhật thương hiệu thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(brand);
        }

        // GET: Admin/Brands/Delete/{id}
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["error"] = "Lỗi: Không tìm thấy mã thương hiệu!";
                return RedirectToAction(nameof(Index));
            }

            var productExists = await _context.Product.Find(p => p.Brand == id).AnyAsync();

            if (productExists)
            {
                TempData["error"] = "Không thể xóa thương hiệu vì đang có sản phẩm thuộc thương hiệu này.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _context.Brand.DeleteOneAsync(b => b.BrandId == id);

            if (result.DeletedCount == 0)
            {
                TempData["error"] = "Không tìm thấy thương hiệu để xóa.";
            }
            else
            {
                TempData["success"] = "Xóa thương hiệu thành công!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
