using AttendanceManagement.API.Models;
using AttendanceManagement.API.Repository.Interfaces;
using MyAttendanceApp.Models;

namespace AttendanceManagement.API.Repository.Implementations
{
    public class WorkerRepository : IWorkerRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public WorkerRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CheckInAsync(int workerId, string barcodeValue)
        {
            var barcodeRepo = _unitOfWork.Repository<Barcode>();
            var attendanceRepo = _unitOfWork.Repository<AttendanceRecord>();

            var barcodes = await barcodeRepo.FindAsync(b => b.BarcodeValue == barcodeValue);
            var barcode = barcodes.FirstOrDefault();
            if (barcode == null) throw new Exception("Barcode not found.");

            // Check if worker already checked in without checking out
            var attends = await attendanceRepo.FindAsync(a => a.WorkerId == workerId );
            var currentlyInside = attends.Any();
            if (currentlyInside) throw new Exception("Worker already checked in.");

            // Record check-in
            var attendance = new AttendanceRecord
            {
                WorkerId = workerId,
                BarcodeId = barcode.BarcodeId,
                CheckIn = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await attendanceRepo.AddAsync(attendance);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CheckOutAsync(int workerId, string barcodeValue)
        {
            var barcodeRepo = _unitOfWork.Repository<Barcode>();
            var attendanceRepo = _unitOfWork.Repository<AttendanceRecord>();

            var barcodes = await barcodeRepo.FindAsync(b => b.BarcodeValue == barcodeValue);
            var barcode = barcodes.FirstOrDefault();
            if (barcode == null) throw new Exception("Barcode not found.");

            // Check if worker is currently checked in
            var attends = await attendanceRepo.FindAsync(a => a.WorkerId == workerId && a.CheckOut == null);
            var currentAttendance = attends.FirstOrDefault();
            if (currentAttendance == null) throw new Exception("No active check-in found for this worker.");

            currentAttendance.CheckOut = DateTime.UtcNow;
            attendanceRepo.Update(currentAttendance);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Absence> CreateAbsenceRequestAsync(int workerId, string reason, DateOnly startDate, DateOnly endDate, AbsenceTypes type)
        {
            var absenceRepo = _unitOfWork.Repository<Absence>();
            var request = new Absence
            {
                WorkerId = workerId,
                Reason = reason,
                StartDate = startDate,
                EndDate = endDate,
                Type = type, // "leave", "absent", "late"
                Status = AbsenceStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            await absenceRepo.AddAsync(request);
            await _unitOfWork.SaveChangesAsync();
            return request;
        }

     
    }
}
