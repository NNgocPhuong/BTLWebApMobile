using System;
using System.Collections.Generic;

namespace Central_server.Data;

public partial class Schedule
{
    public int ScheduleId { get; set; }

    public int? StationId { get; set; }

    public int? ValveId { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string Frequency { get; set; } = null!;

    public virtual Station? Station { get; set; }
}
