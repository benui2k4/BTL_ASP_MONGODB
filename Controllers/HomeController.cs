using ASP_MongoDB.Data;
using ASP_MongoDB.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;

namespace ASP_MongoDB.Controllers
{
    public class HomeController : Controller
    {
        private readonly MongoDBContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, MongoDBContext context)
        {
            _logger = logger;
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statusCode)
        {
            if (statusCode == 404)
            {
                return View("NotFound");
            }
            else
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
}
