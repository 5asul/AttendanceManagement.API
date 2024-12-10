using MyAttendanceApp.Models;

namespace AttendanceManagement.API.Repository.Interfaces
{
    public interface IAdminRepository
    {
        Task<User> AddWorkerAsync(string name, string email, string password);
        Task AssignCheckInCheckOutAsync(int workerId, DateTime checkInTime, DateTime checkOutTime);
        Task<IEnumerable<Attendance>> GetRealTimeAttendanceAsync();
        Task<IEnumerable<Absence>> GetAllAbsenceRequestsAsync();
        Task ApproveAbsenceRequestAsync(int requestId);
        Task RejectAbsenceRequestAsync(int requestId);
        Task<object> GenerateReportAsync(int? workerId, bool yearly, int year, int? month = null);
        Task<string> GenerateBarcodeCodeAsync();
    }
}
