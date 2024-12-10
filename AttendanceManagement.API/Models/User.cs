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

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        public UserRole Role { get; set; } = UserRole.worker;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Barcode> Barcodes { get; set; } = new List<Barcode>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<Absence> Absences { get; set; } = new List<Absence>();
    }
}
