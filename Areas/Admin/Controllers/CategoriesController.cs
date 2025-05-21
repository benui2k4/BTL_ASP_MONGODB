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
    public class CategoriesController : Controller
    {
        private readonly MongoDBContext _context;

        public CategoriesController(MongoDBContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(string searchTerm, int pg = 1)
        {
            const int pageSize = 5;  // Số danh mục mỗi trang

            // Tạo filter cho tìm kiếm
            var filter = Builders<Categories>.Filter.Empty;  // Mặc định là tất cả
            if (!string.IsNullOrEmpty(searchTerm))
            {
                filter = Builders<Categories>.Filter.Regex(c => c.CategoryName, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"));
            }

            // Lấy tổng số danh mục thỏa mãn điều kiện tìm kiếm
            var totalItems = await _context.Category.CountDocumentsAsync(filter);

            // Tạo phân trang
            var pager = new Paginate((int)totalItems, pg, pageSize);

            // Lấy danh sách danh mục cho trang hiện tại
            var categories = await _context.Category
                .Find(filter)
                .SortByDescending(c => c.CategoryId)
                .Skip((pg - 1) * pageSize) // Bỏ qua các mục trước đó
                .Limit(pageSize)           // Lấy số lượng theo page size
                .ToListAsync();

            ViewBag.Pager = pager;
            ViewBag.SearchTerm = searchTerm;

            return View(categories);
        }



        // GET: Admin/Categories/Create 
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Categories category)
        {
            if (ModelState.IsValid)
            {
                
                var existingCategory = await _context.Category
                    .Find(c => c.CategoryName.ToLower() == category.CategoryName.ToLower())
                    .FirstOrDefaultAsync();

                if (existingCategory != null)
                {
                    ModelState.AddModelError("CategoryName", "Tên danh mục đã tồn tại.");
                    return View(category);
                }

                await _context.Category.InsertOneAsync(category);
                TempData["success"] = "Thêm mới danh mục thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }


        // GET: Admin/Categories/Edit/{id}
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["error"] = "Lỗi: Không tìm thấy mã danh mục.";
                return RedirectToAction(nameof(Index));
            }


            var category = await _context.Category.Find(c => c.CategoryId == id).FirstOrDefaultAsync();

            if (category == null)
            {
                TempData["error"] = "Lỗi: Không tìm thấy danh mục.";
                return RedirectToAction(nameof(Index));
            }


            return View(category);
        }

        // POST: Admin/Categories/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Categories category)
        {
            if (id != category.CategoryId)
            {
                TempData["error"] = "Lỗi: ID danh mục không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }


            if (ModelState.IsValid)
            {
                // Kiểm tra xem đã có danh mục khác trùng tên chưa
                var existingCategory = await _context.Category
                    .Find(c => c.CategoryName.ToLower() == category.CategoryName.ToLower() && c.CategoryId != id)
                    .FirstOrDefaultAsync();

                if (existingCategory != null)
                {
                    ModelState.AddModelError("CategoryName", "Tên danh mục đã tồn tại. Vui lòng thử lại!");
                    return View(category);
                }

                var filter = Builders<Categories>.Filter.Eq(c => c.CategoryId, id);
                var update = Builders<Categories>.Update.Set(c => c.CategoryName, category.CategoryName);

                await _context.Category.UpdateOneAsync(filter, update);

                TempData["success"] = "Cập nhật danh mục thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["error"] = "Lỗi: Không tìm thấy mã danh mục!";
                return RedirectToAction("Index");
            }

            
            var productExists = await _context.Product.Find(p => p.Category == id).AnyAsync();

            if (productExists)
            {
                TempData["error"] = "Không thể xóa danh mục vì đang có sản phẩm thuộc danh mục này.";
                return RedirectToAction(nameof(Index));
            }

            // Xóa nếu không có sản phẩm liên kết
            var result = await _context.Category.DeleteOneAsync(c => c.CategoryId == id);

            if (result.DeletedCount == 0)
            {
                TempData["error"] = "Không tìm thấy danh mục để xóa.";
            }
            else
            {
                TempData["success"] = "Xóa danh mục thành công!";
            }

            return RedirectToAction(nameof(Index));
        }





    }
}
