using MyAttendanceApp.Models;

namespace AttendanceManagement.API.Repository.Interfaces
{
    public interface IWorkerRepository
    {
        Task CheckInAsync(int workerId, string barcodeValue);
        Task CheckOutAsync(int workerId, string barcodeValue);
        Task<Absence> CreateAbsenceRequestAsync(int workerId, string reason, DateOnly startDate, DateOnly endDate, AbsenceTypes type);
        // type could be "leave", "absent", or "late"
    }
}
