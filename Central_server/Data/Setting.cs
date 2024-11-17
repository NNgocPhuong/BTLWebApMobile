using System;
using System.Collections.Generic;

namespace Central_server.Data;

public partial class Setting
{
    public int SettingId { get; set; }

    public string SettingType { get; set; } = null!;

    public string SettingUrl { get; set; } = null!;
}
