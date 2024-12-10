using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAttendanceApp.Models

{
    public enum AbsenceStatus
    {
        Approved,
        Rejected,
        Pending
    }

    public enum AbsenceTypes
    {
        Leave,
        Absent,
        Late
    }

    public class Absence
    {
        [Key]
        public int AbsenceId { get; set; }

        [ForeignKey(nameof(Worker))]
        public int WorkerId { get; set; }

        
        public AbsenceTypes Type { get; set; } = AbsenceTypes.Absent;

        [Required]
        public string Reason { get; set; } = null!;

        public AbsenceStatus Status { get; set; } = AbsenceStatus.Pending;

        [Required]
        public DateOnly StartDate { get; set; }
        [Required]
        public DateOnly EndDate { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public User Worker { get; set; } = null!;
    }
}
