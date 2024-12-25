using MyAttendanceApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceManagement.API.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
        [ForeignKey(nameof(Admin))]
        public int AdminId { get; set; } = 1;
        public User Admin { get; set; } = null!;
        [Required]
        public string? FullName { get; set; }
        [Required]
        public string? PhoneNumber { get; set; }
        [Required]
        public string? Password { get; set; }
        public string Location { get; set; } = "DefualtLocation";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //relation one to many
        
        public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();

        // Navigation property for one-to-many relationship in junction table for many to many relation ship
        public virtual ICollection<UserWorkTime> UserWorkTimes { get; set; } = new HashSet<UserWorkTime>();

    }
}
