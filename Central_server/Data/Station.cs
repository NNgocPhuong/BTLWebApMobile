using System;
using System.Collections.Generic;

namespace Central_server.Data;

public partial class Station
{
    public int StationId { get; set; }

    public string Location { get; set; } = null!;

    public bool? Status { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<SensorsDatum> SensorsData { get; set; } = new List<SensorsDatum>();
}
