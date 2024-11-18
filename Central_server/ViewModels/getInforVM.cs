using Central_server.Data;
using System.ComponentModel.DataAnnotations;

namespace Central_server.ViewModels
{
    public class getInforVM
    {
       
        [Display(Name = "Nhiệt độ")]
        public double Temperature { get; set; }
        [Display(Name = "Độ ẩm")]
        public double Humidity { get; set; }

        //[Display(Name = "Trạng thái van")]
        //public bool ValveState { get; set; } // true: mở, false: đóng

        public virtual ICollection<SensorsDatum> SensorsData { get; set; } = new List<SensorsDatum>();
    }
}
