using Central_server.Data;
using System.ComponentModel.DataAnnotations;

namespace Central_server.ViewModels
{
    public class StationDetailVM
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
        public virtual ICollection<Valf> Valves { get; set; } = new List<Valf>();
    }
}
