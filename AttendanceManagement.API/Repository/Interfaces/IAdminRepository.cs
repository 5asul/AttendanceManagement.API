using AttendanceManagement.API.Models;
using MyAttendanceApp.Models;

namespace AttendanceManagement.API.Repository.Interfaces
{
    public interface IAdminRepository
    {
        Task<User> AddWorkerAsync(string name, string email, string password);
        Task AddCheckInCheckOutTimeAsync( DateTime checkInTime, DateTime checkOutTime);
        Task AssignUserToWorkTimeAsync (int user, int workTime);
        Task<IEnumerable<AttendanceRecord>> GetRealTimeAttendanceAsync();
        Task<IEnumerable<Absence>> GetAllAbsenceRequestsAsync();
        Task ApproveAbsenceRequestAsync(int requestId);
        Task RejectAbsenceRequestAsync(int requestId);
        Task UpdateAttendanceStatusAsync(AttendanceStatus attendanceStatus, int attendanceId);    
        Task<object> GenerateReportAsync(int? workerId, bool yearly, int year, int? month = null);
        Task<(string Code, string QrCodeBase64)> GenerateBarcodeCodeAsync();
    }
}
