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

        public virtual ICollection<SensorsDatum> SensorsData { get; set; } = new List<SensorsDatum>();
        public virtual List<valve> Valves { get; set; } = new List<valve>();
    }
}
