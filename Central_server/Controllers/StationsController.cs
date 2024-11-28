using Central_server.Data;
using Central_server.Filters;
using Central_server.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace Central_server.Controllers
{
    public class StationsController : Controller
    {
        private readonly FarmTrackingContext _context;

        public StationsController(FarmTrackingContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            // Load stations from the database asynchronously
            var stationsTask = _context.Stations.Include(s => s.SensorsData).ToListAsync();

            // Await both tasks to complete
            await stationsTask;
            var stations = stationsTask.Result;

            var result = stations.Select(x => new StationDataVM
            {
                StationId = x.StationId,
                Location = x.Location,
                Status = x.Status,
                Temperature = x.SensorsData.OrderByDescending(s => s.Timestamp).FirstOrDefault()?.Temperature ?? 0,
                Humidity = x.SensorsData.OrderByDescending(s => s.Timestamp).FirstOrDefault()?.Humidity ?? 0
            });
            return View(result);
        }
        [Authorize]
        public async Task<IActionResult> Detail(int? id)
        {
            var station = await _context.Stations
                .Include(s => s.SensorsData)
                .Include(s => s.Valves)
                .FirstOrDefaultAsync(s => s.StationId == id);
            if (station == null)
            {
                return NotFound();

            }

            var stationDetail = new StationDetailVM
            {
                StationId = station.StationId,
                Location = station.Location,
                Status = station.Status,
                Temperature = station.SensorsData.OrderByDescending(s => s.Timestamp).FirstOrDefault()?.Temperature ?? 0,
                Humidity = station.SensorsData.OrderByDescending(s => s.Timestamp).FirstOrDefault()?.Humidity ?? 0,
                Valves = station.Valves
            };

            return View(stationDetail);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ControlValve(int? id)
        {
            if (id == 0)
            {
                return BadRequest(new { statusMessage = "ID không hợp lệ" });
            }

            var valve = await _context.Valves.FindAsync(id);
            if (valve == null)
            {
                return NotFound(new { statusMessage = "Valve không tìm thấy" });
            }

            // Kiểm tra trạng thái hiện tại và chuyển đổi trạng thái
            var actionValue = valve.Status == "1" ? 2 : 3;
            var valveId = int.Parse(valve.ValveName.Split(' ').Last());
            var controlMessage = new
            {
                action = "control",
                value = new
                {
                    valves = new[] { new[] { valveId, actionValue } }
                }
            };

            var jsonMessage = JsonConvert.SerializeObject(controlMessage);
            var content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync("http://172.20.10.4/api", content);
                if (response.IsSuccessStatusCode)
                {
                    // Cập nhật trạng thái trong DB
                    valve.Status = actionValue == 3 ? "1" : "0";
                    _context.Update(valve);
                    await _context.SaveChangesAsync();

                    return Ok(new { statusMessage = actionValue == 3 ? "Bật" : "Tắt" });
                }
            }

            return StatusCode(500, new { statusMessage = "Lỗi khi điều khiển Valve" });
        }


    }
}
