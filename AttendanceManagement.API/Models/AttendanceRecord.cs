using MyAttendanceApp.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AttendanceManagement.API.Models
{
    public class AttendanceRecord
    {
        [Key]
        public int AttendanceId { get; set; }

        [ForeignKey(nameof(Worker))]
        public int WorkerId { get; set; }

        [ForeignKey(nameof(Barcode))]
        public int BarcodeId { get; set; }

        [Required]
        public DateTime CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User Worker { get; set; } = null!;
        public Barcode Barcode { get; set; } = null!;
    }
}
