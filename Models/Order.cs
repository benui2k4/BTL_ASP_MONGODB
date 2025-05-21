using ASP_MongoDB.Models.Enum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ASP_MongoDB.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string OrderId { get; set; }
        [BsonElement("FullName")]
        [Required]
        public string FullName { get; set; }

        [BsonElement("EmailAddress")]
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [BsonElement("Phone")]
        [Required]
        [Phone]
        public string Phone { get; set; }

        [BsonElement("Address")]
        [Required]
        public string Address { get; set; }

        [BsonElement("Price")]
        [Required]
        public Double Price { get; set; }


        [BsonElement("Status")]
        [BsonRepresentation(BsonType.String)]
        public OrderStatus Status { get; set; } = OrderStatus.ChoDuyet;


        [BsonElement("CreateAt")]
        public DateTime CreateAt { get; set; }

        public List<OrderItem> OrderItems { get; set; }

    }
}
