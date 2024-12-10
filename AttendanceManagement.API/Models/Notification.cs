using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAttendanceApp.Models
{
    public enum NotificationStatus
    {
        sent,
        read,
        pending
    }

    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [ForeignKey(nameof(Worker))]
        public int WorkerId { get; set; }

        [Required]
        public string Message { get; set; } = null!;

        [Required]
        public NotificationStatus Status { get; set; } = NotificationStatus.pending;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public User Worker { get; set; } = null!;
    }
}
