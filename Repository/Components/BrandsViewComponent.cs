using ASP_MongoDB.Data;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace ASP_MongoDB.Components
{
    public class BrandsViewComponent : ViewComponent
    {
        private readonly MongoDBContext _context;

        public BrandsViewComponent(MongoDBContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var brands = await _context.Brand.Find(_ => true).ToListAsync();
            return View(brands);
        }
    }
}
