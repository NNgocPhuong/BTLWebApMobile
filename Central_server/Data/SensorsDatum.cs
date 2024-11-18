using System;
using System.Collections.Generic;

namespace Central_server.Data;

public partial class SensorsDatum
{
    public int DataId { get; set; }

    public int? StationId { get; set; }

    public double Temperature { get; set; }

    public double Humidity { get; set; }

    public DateTime? Timestamp { get; set; }

    public virtual Station? Station { get; set; }
}
