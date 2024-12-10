using AttendanceManagement.API.Repository.Interfaces;
using MyAttendanceApp.Models;


public class AdminRepository : IAdminRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public AdminRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<User> AddWorkerAsync(string name, string email, string password)
    {
        var userRepo = _unitOfWork.Repository<User>();
        var existingUsers = await userRepo.FindAsync(u => u.Email == email);
        if (existingUsers.Any()) throw new Exception("Email already exists.");

        var worker = new User
        {
            Name = name,
            Email = email,
            Password = password, // In production, hash this
            Role = UserRole.worker,
            CreatedAt = DateTime.UtcNow
        };

        await userRepo.AddAsync(worker);
        await _unitOfWork.SaveChangesAsync();
        return worker;
    }

    public async Task AssignCheckInCheckOutAsync(int workerId, DateTime checkInTime, DateTime checkOutTime)
    {

        var attendanceRepo = _unitOfWork.Repository<Attendance>();

        var newAttendance = new Attendance
        {
            WorkerId = workerId,
            CheckIn = checkInTime,
            CheckOut = checkOutTime,
            CreatedAt = DateTime.UtcNow,
            
        };

        await attendanceRepo.AddAsync(newAttendance);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<Attendance>> GetRealTimeAttendanceAsync()
    {
        // For real-time, you might filter today's attendance or last known check-ins without check-outs
        var attendanceRepo = _unitOfWork.Repository<Attendance>();
        // For simplicity, let's return all today's attendance
        var today = DateTime.UtcNow.Date;
        var records = await attendanceRepo.FindAsync(a => a.CheckIn.Date == today);
        return records;
    }

    public async Task<IEnumerable<Absence>> GetAllAbsenceRequestsAsync()
    {
        var absenceRepo = _unitOfWork.Repository<Absence>();
        return await absenceRepo.GetAllAsync();
    }

    public async Task ApproveAbsenceRequestAsync(int requestId)
    {
        var absenceRepo = _unitOfWork.Repository<Absence>();
        var request = await absenceRepo.GetByIdAsync(requestId);
        if (request == null) throw new Exception("Request not found.");

        request.Status = AbsenceStatus.Approved;
        absenceRepo.Update(request);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RejectAbsenceRequestAsync(int requestId)
    {
        var absenceRepo = _unitOfWork.Repository<Absence>();
        var request = await absenceRepo.GetByIdAsync(requestId);
        if (request == null) throw new Exception("Request not found.");

        request.Status = AbsenceStatus.Rejected;
        absenceRepo.Update(request);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<object> GenerateReportAsync(int? workerId, bool yearly, int year, int? month = null)
    {
        // Simplified reporting logic
        // Retrieve attendance records for either a specific worker or all workers
        var attendanceRepo = _unitOfWork.Repository<Attendance>();
        IEnumerable<Attendance> records;

        if (workerId.HasValue)
        {
            records = await attendanceRepo.FindAsync(a => a.WorkerId == workerId.Value);
        }
        else
        {
            records = await attendanceRepo.GetAllAsync();
        }

        // Filter by year (and month if given)
        records = records.Where(a => a.CheckIn.Year == year);
        if (!yearly && month.HasValue)
        {
            records = records.Where(a => a.CheckIn.Month == month.Value);
        }

        // Return a simple summary
        var totalHours = records.Sum(r => (r.CheckOut.HasValue ? r.CheckOut.Value - r.CheckIn : TimeSpan.Zero).TotalHours);
        return new
        {
            WorkerId = workerId,
            Year = year,
            Month = month,
            TotalHours = totalHours,
            RecordCount = records.Count()
        };
    }

    public async Task<string> GenerateBarcodeCodeAsync()
    {
        // Generate a unique code
        var code = Guid.NewGuid().ToString("N");
        // You might store this in the database as a Barcode entity or return it directly
        var barcodeRepo = _unitOfWork.Repository<Barcode>();
        var barcode = new Barcode
        {
            AdminId = 1, // Replace with actual AdminId from context
            Location = "DefaultLocation",
            BarcodeValue = code,
            CreatedAt = DateTime.UtcNow
        };
        await barcodeRepo.AddAsync(barcode);
        await _unitOfWork.SaveChangesAsync();
        return code;
    }
}
