using ASP_MongoDB.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ASP_MongoDB.Data
{
    public class MongoDBContext
    {
        private readonly IMongoDatabase _database;

        public MongoDBContext(IOptions<MongoDBSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }


        public IMongoDatabase Database => _database;

        public IMongoCollection<Categories> Category => _database.GetCollection<Categories>("Category");

        public IMongoCollection<Brands> Brand => _database.GetCollection<Brands>("Brands");

        public IMongoCollection<Product> Product => _database.GetCollection<Product>("Product");

        public IMongoCollection<ApplicationUser> ApplicationUsers => _database.GetCollection<ApplicationUser>("Users");


        public IMongoCollection<Order> Order => _database.GetCollection<Order>("Order");
    }
}
