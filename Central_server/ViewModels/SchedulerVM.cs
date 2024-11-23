using System.ComponentModel.DataAnnotations;

namespace Central_server.ViewModels
{
    public class SchedulerVM
    {
        public int? ValveId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

    }
}
