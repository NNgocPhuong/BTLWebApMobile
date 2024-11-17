using System.ComponentModel.DataAnnotations;

namespace Central_server.ViewModels
{
    public class LoginVM
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; } = null!;
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = null!;
    }
}
