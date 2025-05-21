using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ASP_MongoDB.Models
{
    public class Categories
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string CategoryId { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống!")]

        [BsonElement("CategoryName")]
        public string CategoryName { get; set; }
    }
}
