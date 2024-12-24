using AttendanceManagement.API.Models;
using MyAttendanceApp.Models;

namespace AttendanceManagement.API.Repository.Interfaces
{
    public interface IAdminRepository
    {
        Task<Employee> AddWorkerAsync(string name, string phoneNumber, string password);
        Task AddCheckInCheckOutTimeAsync( DateTime checkInTime, DateTime checkOutTime, CheckType type);
        Task AssignEmployeeToWorkTimeAsync(int user, int workTime);
        Task<IEnumerable<AttendanceRecord>> GetRealTimeAttendanceAsync();
        Task<IEnumerable<Absence>> GetAllAbsenceRequestsAsync();
        Task ApproveAbsenceRequestAsync(int requestId);
        Task RejectAbsenceRequestAsync(int requestId);
        Task UpdateAttendanceStatusAsync(AttendanceStatus attendanceStatus, int attendanceId);    
        Task<object> GenerateReportAsync(int? workerId, bool yearly, int year, int? month = null);
        Task<(string Code, string QrCodeBase64)> GenerateBarcodeCodeAsync();
    }
}
