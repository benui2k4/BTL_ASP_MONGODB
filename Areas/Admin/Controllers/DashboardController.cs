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
    public class DashboardController : Controller
    {
        private readonly MongoDBContext _context;

        public DashboardController(MongoDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categoryCount = await _context.Category.CountDocumentsAsync(Builders<Categories>.Filter.Empty);
            var brandCount = await _context.Brand.CountDocumentsAsync(Builders<Brands>.Filter.Empty);
            var productCount = await _context.Product.CountDocumentsAsync(Builders<Product>.Filter.Empty);
            var userCount = await _context.ApplicationUsers.CountDocumentsAsync(Builders<ApplicationUser>.Filter.Empty);

            ViewBag.CategoryCount = categoryCount;
            ViewBag.BrandCount = brandCount;
            ViewBag.ProductCount = productCount;
            ViewBag.UserCount = userCount;

            return View();
        }
    }
}
