using Central_server.Data;

namespace Central_server.ViewModels
{
    public class ValveScheduleRequest
    {
        public List<SchedulerVM> Schedules { get; set; } = new List<SchedulerVM>();
    }
}
