using AttendanceManagement.API.Models;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
// [Authorize(Roles = "admin")] // Ideally you'd have authorization here
public class AdminController : ControllerBase
{
    
    private readonly IUnitOfWork _unitOfWork;

    public AdminController( IUnitOfWork unitOfWork)
    {
        
        _unitOfWork = unitOfWork;
    }

    [HttpPost("add-worker")]
    public async Task<IActionResult> AddWorker([FromBody] AddWorkerDto dto)
    {
        var worker = await _unitOfWork.AdminRepository.AddWorkerAsync(dto.Name, dto.PhoneNumber, dto.Password);
        return Ok(worker);
    }

    [HttpPost("assign-worker-to-worke-time")]
    public async Task<IActionResult> AssignUserToWorkTime([FromBody] AssignWorkerToWorkTimeDto dto )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _unitOfWork.AdminRepository.AssignUserToWorkTimeAsync(dto.User, dto.WorkTime);
            return Ok(new { Message = "Worker assigned to work time successfully" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { Error = ex.Message });
        }
        catch (Exception)
        {
            // Log exception as needed
            return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "An error occurred. Please try again later." });
        }

    }

    [HttpPost("add-checkin-checkout-time")]
    public async Task<IActionResult> AddCheckInCheckOutTime([FromBody] AssignCheckInOutDto dto)
    {
        await _unitOfWork.AdminRepository.AddCheckInCheckOutTimeAsync( dto.CheckInTime, dto.CheckOutTime);
        return Ok("Times assigned.");
    }

    [HttpGet("realtime-attendance")]
    public async Task<IActionResult> GetRealTimeAttendance()
    {
        var records = await _unitOfWork.AdminRepository.GetRealTimeAttendanceAsync();
        return Ok(records);
    }

    [HttpGet("absence-requests")]
    public async Task<IActionResult> GetAllAbsenceRequests()
    {
        var requests = await _unitOfWork.AdminRepository.GetAllAbsenceRequestsAsync();
        return Ok(requests);
    }

    [HttpPut("absence-requests/{requestId}/approve")]
    public async Task<IActionResult> ApproveAbsenceRequest(int requestId)
    {
        await _unitOfWork.AdminRepository.ApproveAbsenceRequestAsync(requestId);
        return Ok("Request approved.");
    }

    [HttpPut("absence-requests/{requestId}/reject")]
    public async Task<IActionResult> RejectAbsenceRequest(int requestId)
    {
        await _unitOfWork.AdminRepository.RejectAbsenceRequestAsync(requestId);
        return Ok("Request rejected.");
    }

    [HttpPost("attendance-record-status")]
    public async Task<IActionResult> UpdateAttendanceStatus(AttendanceStatus attendanceStatus , int attendanceId)
    {
        await _unitOfWork.AdminRepository.UpdateAttendanceStatusAsync(attendanceStatus, attendanceId);
        return Ok("Attendance Status Updated.");
    }

    [HttpGet("reports")]
    public async Task<IActionResult> GenerateReport([FromQuery] int? workerId, [FromQuery] bool yearly, [FromQuery] int year, [FromQuery] int? month = null)
    {
        var report = await _unitOfWork.AdminRepository.GenerateReportAsync(workerId, yearly, year, month);
        return Ok(report);
    }

    [HttpPost("generate-barcode-code")]
    public async Task<IActionResult> GenerateBarcodeCode()
    {
        var (barcodeCode, qrCodeBase64) = await _unitOfWork.AdminRepository.GenerateBarcodeCodeAsync();
        return Ok(new { BarcodeCode = barcodeCode, QrCodeBase64 = qrCodeBase64 });
    }
}

// DTOs for AdminController
public class AddWorkerDto
{
    public string Name { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class AssignCheckInOutDto
{
    public DateTime CheckInTime { get; set; }
    public DateTime CheckOutTime { get; set; }

}
public class AssignWorkerToWorkTimeDto
{
    public int User { get; set; }
    public int WorkTime { get; set; }
}
