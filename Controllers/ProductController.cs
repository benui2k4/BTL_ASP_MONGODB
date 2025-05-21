using ASP_MongoDB.Data;
using ASP_MongoDB.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ASP_MongoDB.Controllers
{
    public class ProductController : Controller
    {
        private readonly MongoDBContext _context;

        public ProductController(MongoDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy danh sách sản phẩm
            var products = await _context.Product.Find(_ => true).Limit(21).ToListAsync();

            // Lấy thông tin Category và Brand để ánh xạ tên
            var categories = await _context.Category.Find(_ => true).ToListAsync();
            var brands = await _context.Brand.Find(_ => true).ToListAsync();

            foreach (var product in products)
            {
                product.CategoryName = categories.FirstOrDefault(c => c.CategoryId == product.Category)?.CategoryName ?? "";
                product.BrandName = brands.FirstOrDefault(b => b.BrandId == product.Brand)?.BrandName ?? "";
            }

            return View(products);
        }

        public async Task<IActionResult> Details(string id, int page = 1)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Index");

            var product = await _context.Product.Find(p => p.ProductId == id).FirstOrDefaultAsync();
            if (product == null) return NotFound();

            // Lấy dữ liệu danh mục và thương hiệu cho sản phẩm chính
            var categoryTask = _context.Category.Find(c => c.CategoryId == product.Category).FirstOrDefaultAsync();
            var brandTask = _context.Brand.Find(b => b.BrandId == product.Brand).FirstOrDefaultAsync();

            var category = await categoryTask;
            var brand = await brandTask;

            // Gán tên danh mục và thương hiệu cho sản phẩm chính
            product.CategoryName = category?.CategoryName;
            product.BrandName = brand?.BrandName;

            // Lấy sản phẩm liên quan cùng danh mục với phân trang
            int pageSize = 3;  // Số sản phẩm hiển thị mỗi trang
            var skip = (page - 1) * pageSize;

            var relatedProducts = await _context.Product
                .Find(p => p.Category == product.Category && p.ProductId != product.ProductId)
                .Skip(skip)
                .Limit(pageSize)
                .ToListAsync();

            // Tính tổng số trang
            var totalRelatedProducts = await _context.Product
                .CountDocumentsAsync(p => p.Category == product.Category && p.ProductId != product.ProductId);
            var totalPages = (int)Math.Ceiling(totalRelatedProducts / (double)pageSize);

            // Lấy danh mục và thương hiệu cho các sản phẩm liên quan
            var categoryIds = relatedProducts.Select(p => p.Category).Distinct().ToList();
            var brandIds = relatedProducts.Select(p => p.Brand).Distinct().ToList();

            var categories = await _context.Category.Find(c => categoryIds.Contains(c.CategoryId)).ToListAsync();
            var brands = await _context.Brand.Find(b => brandIds.Contains(b.BrandId)).ToListAsync();

            // Gán tên danh mục và thương hiệu cho từng sản phẩm liên quan
            foreach (var related in relatedProducts)
            {
                related.CategoryName = categories.FirstOrDefault(c => c.CategoryId == related.Category)?.CategoryName;
                related.BrandName = brands.FirstOrDefault(b => b.BrandId == related.Brand)?.BrandName;
            }

            // Gửi dữ liệu sang View
            ViewBag.RelatedProducts = relatedProducts;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.ProductId = id;

            return View(product);
        }


 
        [HttpPost] 
        public async Task<IActionResult> Search(string searchTerm)
        {
            var filter = Builders<Product>.Filter.Or(
                Builders<Product>.Filter.Regex("ProductName", new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                Builders<Product>.Filter.Regex("Description", new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
            );

            var products = await _context.Product.Find(filter).ToListAsync();

            var categoryIds = products.Select(p => p.Category).Distinct().ToList();
            var brandIds = products.Select(p => p.Brand).Distinct().ToList();

            var categories = await _context.Category.Find(c => categoryIds.Contains(c.CategoryId)).ToListAsync();
            var brands = await _context.Brand.Find(b => brandIds.Contains(b.BrandId)).ToListAsync();

            foreach (var product in products)
            {
                product.CategoryName = categories.FirstOrDefault(c => c.CategoryId == product.Category)?.CategoryName;
                product.BrandName = brands.FirstOrDefault(b => b.BrandId == product.Brand)?.BrandName;
            }

            ViewBag.Keyword = searchTerm;
            return View(products);
        }

    }
}
