using Central_server.Data;
using Central_server.ViewModels;
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
            // Call the API asynchronously
            var client = new HttpClient();
            var requestBody = new
            {
                action = "get_infor",
                value = new { }
            };
            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var responseTask = client.PostAsync("https://ducthinh.serveo.net/api", content);

            // Load stations from the database asynchronously
            var stationsTask = _context.Stations.Include(s => s.SensorsData).ToListAsync();

            // Await both tasks to complete
            await Task.WhenAll(responseTask, stationsTask);

            var response = responseTask.Result;
            var stations = stationsTask.Result;

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var stationData = JsonConvert.DeserializeObject<getInforVM>(responseData);

                // Save the data to the database
                var station = await _context.Stations.FindAsync(1);
                if (station != null)
                {
                    //station.Location = stationData.Location;
                    //station.Status = stationData.Status;
                    station.SensorsData.Add(new SensorsDatum
                    {
                        Temperature = stationData.Temperature,
                        Humidity = stationData.Humidity,
                        Timestamp = DateTime.Now
                    });
                    _context.Stations.Update(station);
                    await _context.SaveChangesAsync();
                }
            }

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
    }
}
