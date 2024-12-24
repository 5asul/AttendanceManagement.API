using MyAttendanceApp.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceManagement.API.Models
{
    public class UserWorkTime //junction table class
    {
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        [ForeignKey(nameof(WorkTime))]
        public int WorkTimeId { get; set; }
        public WorkTime? WorkTime { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
