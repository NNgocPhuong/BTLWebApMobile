using Central_server.Data;
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
    }
}
