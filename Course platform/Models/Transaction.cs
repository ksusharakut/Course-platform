using System;
using System.Collections.Generic;
using Course_platform.Models;

namespace Course_platform;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public int? UserId { get; set; }

    public int Amount { get; set; }

    public string? TransactionType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CourseId { get; set; }

    public int? PlatformFee { get; set; }

    public virtual Course? Course { get; set; }

    public virtual User? User { get; set; }
}
