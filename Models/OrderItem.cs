using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ASP_MongoDB.Models
{
    public class OrderItem
    {
        public ObjectId ProductId { get; set; }

        public Double Price { get; set; }

        public int Quantity { get; set; }

        
        public string ProductName { get; set; }
        
        public string Image { get; set; }

        [BsonIgnore]
        public string BrandName { get; set; }

        [BsonIgnore]
        public string CategoryName { get; set; }


    }
}
