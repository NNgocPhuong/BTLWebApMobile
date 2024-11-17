using System.ComponentModel.DataAnnotations;

namespace Central_server.ViewModels
{
    public class StationDataVM
    {
        public int StationId { get; set; }
        [Required]
        [Display(Name = "Tên trạm")]
        public string Location { get; set; } = null!;
        [Required]
        [Display(Name = "Trạng thái")]
        public bool? Status { get; set; } // true: hoạt động, false: không hoạt động
        [Display(Name = "Nhiệt độ")]
        public double Temperature { get; set; }
        [Display(Name = "Độ ẩm")]
        public double Humidity { get; set; }
        //[Display(Name = "Trạng thái van")]
        //public bool ValveState { get; set; } // true: mở, false: đóng
    }
}
