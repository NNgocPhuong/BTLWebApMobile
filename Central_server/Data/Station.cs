using System;
using System.Collections.Generic;

namespace Central_server.Data;

public partial class Station
{
    public int StationId { get; set; }

    public string Location { get; set; } = null!;

    public bool? Status { get; set; }

    public virtual ICollection<SensorsDatum> SensorsData { get; set; } = new List<SensorsDatum>();

    public virtual ICollection<Valf> Valves { get; set; } = new List<Valf>();
}
