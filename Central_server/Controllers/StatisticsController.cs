using Central_server.Data;
using Central_server.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Central_server.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly FarmTrackingContext _context;

        public StatisticsController(FarmTrackingContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int? id)
        {
            var station = _context.Stations.Find(id);
            if (station == null)
            {
                return NotFound();
            }
            var listSensorData = await _context.SensorsData
                .Where(s => s.StationId == id)
                .OrderByDescending(s => s.Timestamp)
                .Take(100)
                .ToListAsync();
            var result = listSensorData.Select(x => new SensorDataVM
            {
                StationId = x.StationId,
                Temperature = x.Temperature,
                Humidity = x.Humidity,
                Timestamp = x.Timestamp
            });
            
            return View(result);
        }
        public async Task<IActionResult> Filter(int? id, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var station = await _context.Stations.FindAsync(id);
            if (station == null)
            {
                return NotFound();
            }

            var listSensorData = await _context.SensorsData
                .Where(s => s.StationId == id && s.Timestamp >= startDate && s.Timestamp <= endDate)
                .OrderByDescending(s => s.Timestamp)
                .ToListAsync();

            if (!listSensorData.Any())
            {
                return Content("No data available for the selected date range.");
            }

            var result = listSensorData.Select(x => new SensorDataVM
            {
                StationId = x.StationId,
                Temperature = x.Temperature,
                Humidity = x.Humidity,
                Timestamp = x.Timestamp
            });

            return View("Index", result);
        }
    }
}
