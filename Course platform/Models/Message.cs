using System;
using System.Collections.Generic;
using Course_platform.Models;

namespace Course_platform;

public partial class Message
{
    public int MessageId { get; set; }

    public int? UserId { get; set; }

    public string MessageText { get; set; } = null!;

    public DateTime? SentAt { get; set; }

    public int? ChatId { get; set; }

    public bool? Read { get; set; }

    public virtual Chat? Chat { get; set; }

    public virtual User? User { get; set; }
}
