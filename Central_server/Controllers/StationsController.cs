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
        
      
        public async Task<IActionResult> ControlValve(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var valve = await _context.Valves.FindAsync(id);
            if (valve == null)
            {
                return NotFound();
            }
            valve.Status = valve.Status == "on" ? "off" : "on";
            _context.Update(valve);
            await _context.SaveChangesAsync();
            //var actionValue = valve.Status == "on" ? 2 : 3;
            //var controlMessage = new
            //{
            //    action = "control",
            //    value = new
            //    {
            //        valves = new[] { new[] { id, actionValue } }
            //    }
            //};

            //var jsonMessage = JsonConvert.SerializeObject(controlMessage);
            //var content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

            //using (var client = new HttpClient())
            //{
            //    var response = await client.PostAsync("https://ducthinh.serveo.net/api", content);
            //    if (response.IsSuccessStatusCode)
            //    {
            //        var maxRetries = 10;
            //        var delay = 1000; // 1 second
            //        var retries = 0;
            //        bool statusUpdated = false;

            //        while (retries < maxRetries && !statusUpdated)
            //        {
            //            await Task.Delay(delay);
            //            retries++;

            //            var responseContent = await response.Content.ReadAsStringAsync();
            //            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);

            //            if (responseObject != null && responseObject.value != null && responseObject.value.valves != null)
            //            {
            //                var valveStatus = responseObject.value.valves[0][1];
            //                valve.Status = valveStatus == 3 ? "on" : "off";
            //                _context.Update(valve);
            //                await _context.SaveChangesAsync();
            //                statusUpdated = true;
            //            }
            //        }
            //    }
            //}

            return RedirectToAction(nameof(Detail), new { id = valve.StationId });
        }
    }
}
