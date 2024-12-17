using AttendanceManagement.API.Models;
using System.ComponentModel.DataAnnotations;

namespace MyAttendanceApp.Models
{
    public enum UserRole
    {
        admin,
        worker
    }

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
        public UserRole Role { get; set; } = UserRole.worker;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties for one-to-many relationship
        public ICollection<Barcode> Barcodes { get; set; } = new List<Barcode>();
        public ICollection<AttendanceRecord> Attendances { get; set; } = new List<AttendanceRecord>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<Absence> Absences { get; set; } = new List<Absence>();

        // Navigation property for one-to-many relationship in junction table for many to many relation ship
        public virtual ICollection<UserWorkTime> UserWorkTimes { get; set; } = new HashSet<UserWorkTime>();
    }
}
