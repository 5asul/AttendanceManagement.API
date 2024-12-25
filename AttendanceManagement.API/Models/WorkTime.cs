using MyAttendanceApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceManagement.API.Models
{
    public enum CheckType
    {
        CheckIn,
        CheckOut
    }
    public class WorkTime
    {
        [Key]
        public int WorkerTimeId { get; set; }
        [ForeignKey(nameof(Admin))]
        public int AdminId { get; set; }
        public User? Admin {  get; set; }

        public DateTime CheckInTime { get; set; } = DateTime.Now;

        public DateTime CheckOutTime { get; set; } = DateTime.Now;

        public CheckType Type { get; set; } = CheckType.CheckIn;
        public string Location { get; set; } = "DefualtLocation";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for one-to-many relationship in junction table for many to many relation ship
        public virtual ICollection<UserWorkTime> UserWorkTimes { get; set; } = new HashSet<UserWorkTime>();

    }
}
