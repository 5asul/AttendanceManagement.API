using MyAttendanceApp.Models;

namespace AttendanceManagement.API.Models
{
    public class UserWorkTime //junction table class
    {

        public int UserId { get; set; }
        public User? User { get; set; }

        public int WorkTimeId { get; set; }
        public WorkTime? WorkTime { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
