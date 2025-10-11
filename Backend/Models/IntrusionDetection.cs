using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Backend.Models;

public class IntrusionDetection
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string SourceIP { get; set; } = string.Empty;

    [Required]
    public string DestinationIP { get; set; } = string.Empty;

    public int SourcePort { get; set; }
    public int DestinationPort { get; set; }

    [Required]
    public string Protocol { get; set; } = string.Empty;

    [Required]
    public string AttackType { get; set; } = string.Empty;

    public string Payload { get; set; } = string.Empty;
    public int Severity { get; set; } // 1-5, 5 being most critical

    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
    public bool IsResolved { get; set; } = false;

    // Changed to use IdentityUser
    public string? ResolvedById { get; set; }

    [ForeignKey("ResolvedById")]
    public virtual IdentityUser? ResolvedBy { get; set; }

    public DateTime? ResolvedAt { get; set; }
}
