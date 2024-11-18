using System;
using System.Collections.Generic;

namespace Central_server.Data;

public partial class Valf
{
    public int ValveId { get; set; }

    public int? StationId { get; set; }

    public string ValveName { get; set; } = null!;

    public string ValveType { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime? LastUpdated { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual Station? Station { get; set; }
}
