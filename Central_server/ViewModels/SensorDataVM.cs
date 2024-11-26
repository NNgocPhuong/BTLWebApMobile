namespace Central_server.ViewModels
{
    public class SensorDataVM
    {
        public int? StationId { get; set; }

        public double Temperature { get; set; }

        public double Humidity { get; set; }

        public DateTime? Timestamp { get; set; }
    }
}
