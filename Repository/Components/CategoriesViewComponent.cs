using ASP_MongoDB.Data;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace ASP_MongoDB.Repository.Components
{
    public class CategoriesViewComponent : ViewComponent
    {
        private readonly MongoDBContext _context;

        public CategoriesViewComponent(MongoDBContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _context.Category.Find(_ => true).ToListAsync();
            return View(categories);
        }
    }
}
