using Central_server.Data;
using Central_server.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace Central_server.Controllers
{
    public class SchedulerController : Controller
    {
        private readonly FarmTrackingContext _context;

        public SchedulerController(FarmTrackingContext context) { _context = context; }
        [Authorize]
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
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                }).ToList()
            }).ToList();

            return View(valveVM);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddSchedules(ValveScheduleRequest request)
        {
            if (request == null || request.Schedules == null || !request.Schedules.Any())
            {
                return BadRequest("No schedules provided");
            }
            // Lọc các lịch trình hợp lệ (có đủ dữ liệu StartTime và EndTime)
            var validSchedules = request.Schedules
                .Where(s => s.StartTime != DateTime.MinValue && s.EndTime != DateTime.MinValue)
                .ToList();

            if (!validSchedules.Any())
            {
                return BadRequest("No valid schedules provided");
            }

            if (ModelState.IsValid)
            {
                var schedules = validSchedules.Select((s, index) => new object[]
                {
            index + 1, // Valve sequence number
            ConvertToCustomDateTimeFormat(s.StartTime),
            ConvertToCustomDateTimeFormat(s.EndTime)
                }).ToArray();

                var controlMessage = new
                {
                    action = "schedule",
                    value = new
                    {
                        valves = schedules
                    }
                };

                var jsonMessage = JsonConvert.SerializeObject(controlMessage);
                var content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync("https://ducthinh.serveo.net/api", content);
                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        // Optionally, save the schedules to the database
                        foreach (var schedule in request.Schedules)
                        {
                            var newSchedule = new Schedule
                            {
                                ValveId = schedule.ValveId,
                                StartTime = schedule.StartTime,
                                EndTime = schedule.EndTime,
                                Frequency = "Once", // Example frequency, adjust as needed
                                Status = "Scheduled",
                                CreatedAt = DateTime.Now
                            };
                            _context.Schedules.Add(newSchedule);
                        }
                        await _context.SaveChangesAsync();
                    }
                }

                return RedirectToAction("Index", "Stations");
            }

            return View("Index");
        }

        private string ConvertToCustomDateTimeFormat(DateTime dateTime)
        {
            return dateTime.ToString("yyMMddHHmm");
        }

    }
}
