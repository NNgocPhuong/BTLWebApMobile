using Central_server.Data;
using Central_server.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Central_server.Controllers
{
    public class SchedulerController : Controller
    {
        private readonly FarmTrackingContext _context;

        public SchedulerController(FarmTrackingContext context) { _context = context; }
        public IActionResult Index(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var station = _context.Stations.Find(id);
            if (station == null)
            {
                return NotFound();
            }
            // lấy ra các van của trạm và các lịch trình của chúng 
            var valves = _context.Valves.Include(v => v.Schedules).Where(v => v.StationId == id).ToList();
            List<ValveVM> valveVM = valves.Select(v => new ValveVM
            {
                ValveId = v.ValveId,
                ValveName = v.ValveName,
                Status = v.Status,
                Schedules = v.Schedules.Select(s => new SchedulerVM
                {
                    ValveId = s.ValveId,
                    ScheduleId = s.ScheduleId,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Frequency = s.Frequency,
                    Status = s.Status
                }).ToList()
            }).ToList();

            return View(valveVM);
        }
    }
}
