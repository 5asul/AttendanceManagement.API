using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAttendanceApp.Models
{
    public class Attendance
    {
        [Key]
        public int AttendanceId { get; set; }

        [ForeignKey(nameof(Worker))]
        public int WorkerId { get; set; }

        [Required]
        public DateTime CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User Worker { get; set; } = null!;
    }
}
