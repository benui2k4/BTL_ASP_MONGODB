using System.ComponentModel.DataAnnotations;

namespace ASP_MongoDB.Models.Enum
{
    public enum OrderStatus
    {
        [Display(Name = "Chờ duyệt")]
        ChoDuyet,

        [Display(Name = "Đã duyệt")]
        DaDuyet
    }

}
