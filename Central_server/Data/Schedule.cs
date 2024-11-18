using System;
using System.Collections.Generic;

namespace Central_server.Data;

public partial class Schedule
{
    public int ScheduleId { get; set; }

    public int? ValveId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string Frequency { get; set; } = null!;

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Valf? Valve { get; set; }
}
