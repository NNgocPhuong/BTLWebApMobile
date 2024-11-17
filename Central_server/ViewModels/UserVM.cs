using System.ComponentModel.DataAnnotations;

namespace Central_server.ViewModels
{
    public class UserVM
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; } = null!;
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = null!;
        [Required(ErrorMessage = "Không được để trống")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = null!;
        [Required(ErrorMessage = "Không được để trống")]
        [Display(Name = "Mã số sinh viên")]
        public string StudentId { get; set; } = null!;
        [Display(Name = "Ảnh đại diện")]
        public IFormFile? Photo { get; set; }
        [Display(Name = "Ảnh đại diện")]
        public string? PhotoUrl { get; set; }
    }
}
