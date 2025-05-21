using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace ASP_MongoDB.Models;
[CollectionName("Users")]
public class ApplicationUser : MongoIdentityUser<Guid>
{

    [Required(ErrorMessage = "Họ và tên không được để trống!")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Địa chỉ không được để trống!")]
    public string Address { get; set; }


    public bool IsActive { get; set; } = true;


}
