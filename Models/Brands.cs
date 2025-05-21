using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ASP_MongoDB.Models
{
    public class Brands
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string BrandId { get; set; }

        [Required(ErrorMessage = "Tên thương hiệu không được để trống!")]

        [BsonElement("BrandName")]
        public string BrandName { get; set; }
    }
}
