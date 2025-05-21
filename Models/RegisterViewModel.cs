using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ASP_MongoDB.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập Email của bạn!")]
        [EmailAddress(ErrorMessage = "Email của bạn không hợp lệ . Vui lòng kiểm tra lại!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên của bạn!")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập của bạn")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }


        [Required(ErrorMessage = "Vui lòng nhập địa chỉ của bạn!")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại!")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ . Hãy kiểm tra lại định dạng!")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu để bảo mật!")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận lại mật khẩu vừa nhập!")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp. Vui lòng kiểm tra lại!")]
        public string ConfirmPassword { get; set; }

        [HiddenInput(DisplayValue =false)]
        public string Role { get; set; } // "Admin" hoặc "User"
    }
}
