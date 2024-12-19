using AttendanceManagement.API.Models;
using AttendanceManagement.API.Repository.Interfaces;
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

    public async Task<User> AddWorkerAsync(string name, string PhoneNumber, string password)
    {
        var userRepo = _unitOfWork.Repository<User>();
        var existingUsers = await userRepo.FindAsync(u => u.PhoneNumber == PhoneNumber);
        if (existingUsers.Any()) throw new Exception("Email already exists.");

        var worker = new User
        {
            Name = name,
            PhoneNumber = PhoneNumber,
            Password = password, // In production, hash this
            Role = UserRole.worker,
            CreatedAt = DateTime.UtcNow
        };

        await userRepo.AddAsync(worker);
        await _unitOfWork.SaveChangesAsync();
        return worker;
    }

    public async Task AssignUserToWorkTimeAsync(int userId, int workTimeId)
    {
        if (userId <= 0)
            throw new ArgumentException("Invalid user id.", nameof(userId));
        if (workTimeId <= 0)
            throw new ArgumentException("Invalid work time id.", nameof(workTimeId));

        var userWorkTimeRepo = _unitOfWork.Repository<UserWorkTime>();
        var userRepo = _unitOfWork.Repository<User>();
        var workTimeRepo = _unitOfWork.Repository<WorkTime>();

        // Check that the user exists
        var user = await userRepo.GetByIdAsync(userId);
        if (user == null)
            throw new InvalidOperationException($"User with ID {userId} not found.");

        // Check that the work time exists
        var workTime = await workTimeRepo.GetByIdAsync(workTimeId);
        if (workTime == null)
            throw new InvalidOperationException($"WorkTime with ID {workTimeId} not found.");

        // Check if this user is already assigned to the given work time
        bool alreadyAssigned = await userWorkTimeRepo.AnyAsync(uwt => uwt.UserId == userId && uwt.WorkTimeId == workTimeId);
        if (alreadyAssigned)
            throw new InvalidOperationException("This user is already assigned to the specified work time.");


        // Assign the user to the work time
        var newAssignedUserToWorkTime = new UserWorkTime
        {
            User = user,
            WorkTime = workTime,
        };

        await userWorkTimeRepo.AddAsync(newAssignedUserToWorkTime);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AddCheckInCheckOutTimeAsync( DateTime checkInTime, DateTime checkOutTime)
    {

        var WorkTimeRepo = _unitOfWork.Repository<WorkTime>();

        var newWorkTime = new WorkTime
        {

            CheckInTime = checkInTime,
            CheckOutTime = checkOutTime,
            
            
        };

        await WorkTimeRepo.AddAsync(newWorkTime);
        await _unitOfWork.SaveChangesAsync();
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
        // Simplified reporting logic
        // Retrieve attendance records for either a specific worker or all workers
        var attendanceRepo = _unitOfWork.Repository<AttendanceRecord>();
        IEnumerable<AttendanceRecord> records;

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
