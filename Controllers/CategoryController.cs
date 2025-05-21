using ASP_MongoDB.Data;
using ASP_MongoDB.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;

namespace ASP_MongoDB.Controllers
{
    public class CategoryController : Controller
    {
        private readonly MongoDBContext _context;

        public CategoryController(MongoDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index", "Home");

            var category = await _context.Category.Find(c => c.CategoryId == id).FirstOrDefaultAsync();
            if (category == null)
                return RedirectToAction("Index", "Home");

            var filter = Builders<Product>.Filter.Eq(p => p.Category, id);
            var products = await _context.Product.Find(filter).SortByDescending(p => p.ProductId).ToListAsync();

            
            var brands = await _context.Brand.Find(_ => true).ToListAsync();
            foreach (var product in products)
            {
                product.CategoryName = category.CategoryName;
                product.BrandName = brands.FirstOrDefault(b => b.BrandId == product.Brand)?.BrandName ?? "";
            }

            return View(products);
        }
    }
}
