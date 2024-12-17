using MyAttendanceApp.Models;
using System.ComponentModel.DataAnnotations;

namespace AttendanceManagement.API.Models
{
    public class WorkTime
    {
        [Key]
        public int WorkerTimeId { get; set; }

        public DateTime CheckInTime { get; set; } = DateTime.Now;

        public DateTime CheckOutTime { get; set; } = DateTime.Now;
        public string Location { get; set; } = "DefualtLocation";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for one-to-many relationship in junction table for many to many relation ship
        public virtual ICollection<UserWorkTime> UserWorkTimes { get; set; } = new HashSet<UserWorkTime>();

    }
}
