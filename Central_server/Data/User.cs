using System;
using System.Collections.Generic;

namespace Central_server.Data;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string StudentId { get; set; } = null!;

    public string? PhotoUrl { get; set; }

    public string UserName { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public byte[]? Salt { get; set; }
}
