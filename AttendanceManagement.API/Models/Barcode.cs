using AttendanceManagement.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAttendanceApp.Models
{
    public class Barcode
    {
        [Key]
        public int BarcodeId { get; set; }

        [ForeignKey(nameof(Admin))]
        public int AdminId { get; set; }

        [Required]
        public string Location { get; set; } = null!;

        [Required]
        public string BarcodeValue { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public User Admin { get; set; } = null!;
        public ICollection<AttendanceRecord> Attendances { get; set; } = new List<AttendanceRecord>();
    }
}
