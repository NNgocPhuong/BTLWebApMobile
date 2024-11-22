using Central_server.Data;
using System.ComponentModel.DataAnnotations;

namespace Central_server.ViewModels
{
    public class ValveVM
    {
        public int ValveId { get; set; }
        [Required]
        [Display(Name = "Tên van")]
        public string ValveName { get; set; }
        [Required]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; }
        [Required]
        [Display(Name = "Lịch trình")]
        public virtual ICollection<SchedulerVM> Schedules { get; set; } = new List<SchedulerVM>();
    }
}
