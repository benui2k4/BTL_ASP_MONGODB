using System.ComponentModel.DataAnnotations;

namespace ASP_MongoDB.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email để đăng nhập!")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu để đăng nhập!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
