using Central_server.Data;
using Central_server.ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace Central_server.Services
{
    public class StationDataFetcher : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceProvider _serviceProvider;

        public StationDataFetcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(15));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<FarmTrackingContext>();
                var client = new HttpClient
                {
                    Timeout = TimeSpan.FromMinutes(5) // Increase the timeout to 5 minutes
                };
                var requestBody = new
                {
                    action = "get_infor",
                    value = new { }
                };
                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync("http://172.20.10.4/api", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var stationData = JsonConvert.DeserializeObject<getInforVM>(responseData);

                        // Save the data to the database
                        var station = await context.Stations.FindAsync(1);
                        if (station != null)
                        {
                            // Update sensor data
                            station.SensorsData.Add(new SensorsDatum
                            {
                                Temperature = stationData.Temperature,
                                Humidity = stationData.Humidity,
                                Timestamp = DateTime.Now
                            });

                            // Get the list of valves from the database
                            var valves = await context.Valves.Where(v => v.StationId == station.StationId).ToListAsync();

                            // Update valve data
                            for (int i = 0; i < stationData.Valves.Count; i++)
                            {
                                var apiValve = stationData.Valves[i];
                                var dbValve = valves.ElementAtOrDefault(i);

                                if (dbValve == null)
                                {
                                    // Add new valve
                                    dbValve = new Valf
                                    {
                                        StationId = station.StationId,
                                        ValveName = $"Valve {i + 1}", // Assign a default name
                                        ValveType = "pump", // Assign a default type
                                        Status = apiValve.Status,
                                        LastUpdated = DateTime.Now
                                    };
                                    context.Valves.Add(dbValve);
                                }
                                else
                                {
                                    // Update existing valve
                                    dbValve.Status = apiValve.Status;
                                    dbValve.LastUpdated = DateTime.Now;
                                    context.Valves.Update(dbValve);
                                }
                            }

                            context.Stations.Update(station);
                            await context.SaveChangesAsync();
                        }
                    }
                }
                catch (TaskCanceledException ex)
                {
                    // Handle the timeout exception
                    Console.WriteLine($"Request timed out: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Handle other exceptions
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }





        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}

#region Khi có nhiều API từ nhiều trạm cảm biến khác nhau
//using Central_server.Data;
//using Central_server.ViewModels;
//using Newtonsoft.Json;
//using System.Text;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.DependencyInjection;

//namespace Central_server.Services
//{
//    public class StationDataFetcher : IHostedService, IDisposable
//    {
//        private Timer _timer;
//        private readonly IServiceProvider _serviceProvider;
//        private readonly List<string> _apiUrls;

//        public StationDataFetcher(IServiceProvider serviceProvider)
//        {
//            _serviceProvider = serviceProvider;
//            _apiUrls = new List<string>
//            {
//                "https://ducthinh.serveo.net/api/station1",
//                "https://ducthinh.serveo.net/api/station2",
//                "https://ducthinh.serveo.net/api/station3"
//                // Thêm các URL API khác tại đây
//            };
//        }

//        public Task StartAsync(CancellationToken cancellationToken)
//        {
//            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
//            return Task.CompletedTask;
//        }

//        private async void DoWork(object state)
//        {
//            using (var scope = _serviceProvider.CreateScope())
//            {
//                var context = scope.ServiceProvider.GetRequiredService<FarmTrackingContext>();
//                var client = new HttpClient();

//                for (int i = 0; i < _apiUrls.Count; i++)
//                {
//                    var requestBody = new
//                    {
//                        action = "get_infor",
//                        value = new { }
//                    };
//                    var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
//                    var response = await client.PostAsync(_apiUrls[i], content);

//                    if (response.IsSuccessStatusCode)
//                    {
//                        var responseData = await response.Content.ReadAsStringAsync();
//                        var stationData = JsonConvert.DeserializeObject<getInforVM>(responseData);

//                        // Save the data to the database
//                        var station = await context.Stations.FindAsync(i + 1); // ID tăng dần bắt đầu từ 1
//                        if (station != null)
//                        {
//                            station.SensorsData.Add(new SensorsDatum
//                            {
//                                Temperature = stationData.Temperature,
//                                Humidity = stationData.Humidity,
//                                Timestamp = DateTime.Now
//                            });
//                            context.Stations.Update(station);
//                        }
//                    }
//                }

//                await context.SaveChangesAsync();
//            }
//        }

//        public Task StopAsync(CancellationToken cancellationToken)
//        {
//            _timer?.Change(Timeout.Infinite, 0);
//            return Task.CompletedTask;
//        }

//        public void Dispose()
//        {
//            _timer?.Dispose();
//        }
//    }
//}


#endregion
