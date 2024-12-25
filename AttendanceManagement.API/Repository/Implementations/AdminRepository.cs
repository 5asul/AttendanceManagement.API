using AttendanceManagement.API.Models;
using AttendanceManagement.API.Repository.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MyAttendanceApp.Models;
using QRCoder;
using static QRCoder.PayloadGenerator;




public class AdminRepository : IAdminRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public AdminRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Employee> AddEmployeeAsync (string name, string phoneNumber, string password)
    {
        var userRepo = _unitOfWork.Repository<Employee>();
        var existingUsers = await userRepo.FindAsync(u => u.PhoneNumber == phoneNumber);
        if (existingUsers.Any()) throw new Exception("Email already exists.");

        var worker = new Employee
        {
            FullName = name,
            PhoneNumber = phoneNumber,
            Password = password, // In production, hash this
            CreatedAt = DateTime.UtcNow
        };

        await userRepo.AddAsync(worker);
        await _unitOfWork.SaveChangesAsync();
        return worker;
    }

    public async Task<IEnumerable<Employee>> GetEmployeesAsync(int adminId)
    {
        var employeeRepo = _unitOfWork.Repository<Employee>();
        var employees = await employeeRepo.FindAsync(e => e.AdminId == adminId);

        return employees;

    }

    public async Task EditEmployeeInfo(int employeeId, Employee employee)
    {
        var employeeRepo = _unitOfWork.Repository<Employee>();
        var employeeRecord = await employeeRepo.GetByIdAsync(employeeId);
        if (employeeRecord == null) throw new Exception("Attendance Record not found");

        employeeRecord.FullName = employee.FullName;
        employeeRecord.PhoneNumber = employee.PhoneNumber;
        employeeRecord.Password = employee.Password;
        
        employeeRepo.Update(employee);
        await _unitOfWork.SaveChangesAsync();   

        
    }

    public async Task DeleteEmployeeInfoAsync(int employeeId)
    {
        var employeeRepo = _unitOfWork.Repository<Employee>();

        var employeeRecord = await employeeRepo.GetByIdAsync(employeeId);
        if (employeeRecord == null)
            throw new Exception("Employee record not found");

        
        employeeRepo.Remove(employeeRecord);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AssignEmployeeToWorkTimeAsync(int employeeId, int workTimeId)
    {
        if (employeeId <= 0)
            throw new ArgumentException("Invalid user id.", nameof(employeeId));
        if (workTimeId <= 0)
            throw new ArgumentException("Invalid work time id.", nameof(workTimeId));

        var userWorkTimeRepo = _unitOfWork.Repository<UserWorkTime>();
        var userRepo = _unitOfWork.Repository<Employee>();
        var workTimeRepo = _unitOfWork.Repository<WorkTime>();

        // Check that the user exists
        var user = await userRepo.GetByIdAsync(employeeId);
        if (user == null)
            throw new InvalidOperationException($"User with ID {employeeId} not found.");

        // Check that the work time exists
        var workTime = await workTimeRepo.GetByIdAsync(workTimeId);
        if (workTime == null)
            throw new InvalidOperationException($"WorkTime with ID {workTimeId} not found.");

        // Check if this user is already assigned to the given work time
        bool alreadyAssigned = await userWorkTimeRepo.AnyAsync(uwt => uwt.EmployeeId == employeeId && uwt.WorkTimeId == workTimeId);
        if (alreadyAssigned)
            throw new InvalidOperationException("This user is already assigned to the specified work time.");


        // Assign the user to the work time
        var newAssignedUserToWorkTime = new UserWorkTime
        {
            Employee = user,
            WorkTime = workTime,
        };

        await userWorkTimeRepo.AddAsync(newAssignedUserToWorkTime);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AddCheckInCheckOutTimeAsync( DateTime checkInTime, DateTime checkOutTime , CheckType type)
    {

        var WorkTimeRepo = _unitOfWork.Repository<WorkTime>();

        var newWorkTime = new WorkTime
        {

            CheckInTime = checkInTime,
            CheckOutTime = checkOutTime,
            Type = type
            
            
        };

        await WorkTimeRepo.AddAsync(newWorkTime);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<WorkTime>> GetWorkTimeAsync(int adminId)
    {
        var workTimeRepo = _unitOfWork.Repository<WorkTime>();

        var workTime = await workTimeRepo.FindAsync(e => e.AdminId == adminId);

        return workTime;

    }

    public async Task<IEnumerable<AttendanceRecord>> GetRealTimeAttendanceAsync()
    {
        // For real-time, you might filter today's attendance or last known check-ins without check-outs
        var attendanceRepo = _unitOfWork.Repository<AttendanceRecord>();
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

    public async Task UpdateAttendanceStatusAsync(AttendanceStatus attendanceStatus, int attendanceId)
    {
        var attendanceRepo = _unitOfWork.Repository<AttendanceRecord>();
        var attendanceRecord = await attendanceRepo.GetByIdAsync(attendanceId);
        if (attendanceRecord == null) throw new Exception("Attendance Record not found");

        attendanceRecord.Status = attendanceStatus;
        attendanceRepo.Update(attendanceRecord);
        await _unitOfWork.SaveChangesAsync();
    }


    public async Task<object> GenerateReportAsync(int? workerId, bool yearly, int year, int? month = null)
    {
        // Repositories
        var absenceRepo = _unitOfWork.Repository<Absence>();
        var attendanceRepo = _unitOfWork.Repository<AttendanceRecord>();

        // Fetch records based on the workerId
        IEnumerable<Absence> absences;
        IEnumerable<AttendanceRecord> attendanceRecords;

        if (workerId.HasValue)
        {
            absences = await absenceRepo.FindAsync(a => a.WorkerId == workerId.Value);
            attendanceRecords = await attendanceRepo.FindAsync(a => a.WorkerId == workerId.Value);
        }
        else
        {
            absences = await absenceRepo.GetAllAsync();
            attendanceRecords = await attendanceRepo.GetAllAsync();
        }

        // Filter absences and attendance by year (and month if provided)
        absences = absences.Where(a => a.StartDate.Year == year || a.EndDate.Year == year);
        attendanceRecords = attendanceRecords.Where(a => a.CheckIn.Year == year);

        if (!yearly && month.HasValue)
        {
            absences = absences.Where(a => a.StartDate.Month == month.Value || a.EndDate.Month == month.Value);
            attendanceRecords = attendanceRecords.Where(a => a.CheckIn.Month == month.Value);
        }

        // Group and count absences by type and status
        var absenceReport = new
        {
            TotalAbsences = absences.Count(),
            Leave = new
            {
                Total = absences.Count(a => a.Type == AbsenceTypes.Leave),
                Pending = absences.Count(a => a.Type == AbsenceTypes.Leave && a.Status == AbsenceStatus.Pending),
                Approved = absences.Count(a => a.Type == AbsenceTypes.Leave && a.Status == AbsenceStatus.Approved),
                Rejected = absences.Count(a => a.Type == AbsenceTypes.Leave && a.Status == AbsenceStatus.Rejected)
            },
            Absent = new
            {
                Total = absences.Count(a => a.Type == AbsenceTypes.Absent),
                Pending = absences.Count(a => a.Type == AbsenceTypes.Absent && a.Status == AbsenceStatus.Pending),
                Approved = absences.Count(a => a.Type == AbsenceTypes.Absent && a.Status == AbsenceStatus.Approved),
                Rejected = absences.Count(a => a.Type == AbsenceTypes.Absent && a.Status == AbsenceStatus.Rejected)
            },
            Late = new
            {
                Total = absences.Count(a => a.Type == AbsenceTypes.Late),
                Pending = absences.Count(a => a.Type == AbsenceTypes.Late && a.Status == AbsenceStatus.Pending),
                Approved = absences.Count(a => a.Type == AbsenceTypes.Late && a.Status == AbsenceStatus.Approved),
                Rejected = absences.Count(a => a.Type == AbsenceTypes.Late && a.Status == AbsenceStatus.Rejected)
            }
        };

        // Attendance summary
        var totalHours = attendanceRecords.Sum(r => (r.CheckOut.HasValue ? r.CheckOut.Value - r.CheckIn : TimeSpan.Zero).TotalHours);

        var attendanceReport = new
        {
            TotalAttendanceRecords = attendanceRecords.Count(),
            TotalHours = totalHours,
            StatusCounts = new
            {
                Absent = attendanceRecords.Count(a => a.Status == AttendanceStatus.Absent),
                AtWork = attendanceRecords.Count(a => a.Status == AttendanceStatus.AtWork),
                Attended = attendanceRecords.Count(a => a.Status == AttendanceStatus.Attended)
            }
        };

        // Combined report
        var report = new
        {
            WorkerId = workerId,
            Year = year,
            Month = month,
            AbsenceReport = absenceReport,
            AttendanceReport = attendanceReport
        };

        return report;
    }



    //public async Task<object> GenerateReportAsync(int? workerId, bool yearly, int year, int? month = null)
    //{
    //    // Simplified reporting logic
    //    // Retrieve attendance records for either a specific worker or all workers
    //    var attendanceRepo = _unitOfWork.Repository<AttendanceRecord>();
    //    IEnumerable<AttendanceRecord> records;

    //    if (workerId.HasValue)
    //    {
    //        records = await attendanceRepo.FindAsync(a => a.WorkerId == workerId.Value);
    //    }
    //    else
    //    {
    //        records = await attendanceRepo.GetAllAsync();
    //    }

    //    // Filter by year (and month if given)
    //    records = records.Where(a => a.CheckIn.Year == year);
    //    if (!yearly && month.HasValue)
    //    {
    //        records = records.Where(a => a.CheckIn.Month == month.Value);
    //    }

    //    // Return a simple summary
    //    var totalHours = records.Sum(r => (r.CheckOut.HasValue ? r.CheckOut.Value - r.CheckIn : TimeSpan.Zero).TotalHours);
    //    return new
    //    {
    //        WorkerId = workerId,
    //        Year = year,
    //        Month = month,
    //        TotalHours = totalHours,
    //        RecordCount = records.Count()
    //    };
    //}


    public async Task<(string Code, string QrCodeBase64)> GenerateBarcodeCodeAsync()
    {
        // Generate a unique code
        var code = Guid.NewGuid().ToString("N");



        // Generate the base64 barcode
        var qrGenerator = new QRCodeGenerator();

        QRCodeData qrCodeData = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrCodeData);
        byte[] qrCodeBytes = qrCode.GetGraphic(20);

        var qrCodeBase64 = Convert.ToBase64String(qrCodeBytes);

        //save png qrCode
        var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "QRCodes");
        if (!Directory.Exists(imagesFolder))
        {
            Directory.CreateDirectory(imagesFolder);
        }

        var filePath = Path.Combine(imagesFolder, $"{code}.png");
        await File.WriteAllBytesAsync(filePath, qrCodeBytes);


        //save in database
        var barcodeRepo = _unitOfWork.Repository<Barcode>();
        var barcode = new Barcode
        {
            AdminId = 1,
            Location = "DefaultLocation",
            BarcodeValue = code,
            BarcodeBase64 = qrCodeBase64,
            CreatedAt = DateTime.UtcNow
        };
        await barcodeRepo.AddAsync(barcode);
        await _unitOfWork.SaveChangesAsync();

        return (code, qrCodeBase64);
    }

   

   







    //public async Task<string> GenerateBarcodeCodeAsync()
    //{
    //    // Generate a unique code
    //    var code = Guid.NewGuid().ToString("N");
    //    // You might store this in the database as a Barcode entity or return it directly
    //    var barcodeRepo = _unitOfWork.Repository<Barcode>();
    //    var barcode = new Barcode
    //    {
    //        AdminId = 1, // Replace with actual AdminId from context
    //        Location = "DefaultLocation",
    //        BarcodeValue = code,
    //        CreatedAt = DateTime.UtcNow
    //    };
    //    await barcodeRepo.AddAsync(barcode);
    //    await _unitOfWork.SaveChangesAsync();
    //    return code;
    //}
}
