using AttendanceManagement.API.Models;
using System.ComponentModel.DataAnnotations;

namespace MyAttendanceApp.Models
{
  

    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required, Phone]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties for one-to-many relationship
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public ICollection<Barcode> Barcodes { get; set; } = new List<Barcode>();
        public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<Absence> Absences { get; set; } = new List<Absence>();

       
    }
}
