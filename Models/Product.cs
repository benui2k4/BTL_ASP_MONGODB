using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ASP_MongoDB.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }

        [BsonElement("ProductName")]
        [Required(ErrorMessage = "Tên sản phẩm không được để trống!")]
        [BsonRequired]
        public string ProductName { get; set; }

        [BsonElement("Image")]
        public string Image { get; set; }

        [BsonElement("Description")]
        [Required(ErrorMessage = "Mô tả sản phẩm không được để trống!")]
        [BsonRequired]
        public string Description { get; set; }

        [BsonElement("Price")]
        [Required(ErrorMessage = "Giá sản phẩm không được để trống!")]
        [BsonRequired]
        public double? Price { get; set; }

        [BsonElement("Quantity")]
        [Required(ErrorMessage = "Số lượng sản phẩm không được để trống!")]
        [BsonRequired]
        public int? Quantity { get; set; }

        [BsonElement("Category")]
        [BsonRepresentation(BsonType.ObjectId)]
        [Required(ErrorMessage = "Lựa chọn danh mục sản phẩm là bắt buộc!")]
        public string Category { get; set; }

        [BsonIgnore]  
        public string CategoryName { get; set; }

        [BsonElement("Brand")]
        [BsonRepresentation(BsonType.ObjectId)]
        [Required(ErrorMessage = "Lựa chọn thương hiệu sản phẩm là bắt buộc!")]
        public string Brand { get; set; }

        [BsonIgnore]
        public string BrandName { get; set; }
    }
}
