using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Backend.Models
{
    public class Alert
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public int Severity { get; set; }
        public string AlertType { get; set; } = string.Empty; // Intrusion, System, Security
        public bool IsAcknowledged { get; set; } = false;

        // Changed to use IdentityUser
        public string? AcknowledgedById { get; set; }

        [ForeignKey("AcknowledgedById")]
        public virtual IdentityUser? AcknowledgedBy { get; set; }

        public DateTime? AcknowledgedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
