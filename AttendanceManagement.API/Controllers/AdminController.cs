using AttendanceManagement.API.Models;
using AttendanceManagement.API.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
// [Authorize(Roles = "admin")] // Ideally you'd have authorization here
public class AdminController : ControllerBase
{
    private readonly IAdminRepository _adminRepository;

    public AdminController(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    [HttpPost("add-worker")]
    public async Task<IActionResult> AddWorker([FromBody] AddWorkerDto dto)
    {
        var worker = await _adminRepository.AddWorkerAsync(dto.Name, dto.Email, dto.Password);
        return Ok(worker);
    }

    [HttpPost("assign-checkin-checkout")]
    public async Task<IActionResult> AssignCheckInCheckOut([FromBody] AssignCheckInOutDto dto)
    {
        await _adminRepository.AssignCheckInCheckOutAsync(dto.WorkerId, dto.CheckInTime, dto.CheckOutTime);
        return Ok("Times assigned.");
    }

    [HttpGet("realtime-attendance")]
    public async Task<IActionResult> GetRealTimeAttendance()
    {
        var records = await _adminRepository.GetRealTimeAttendanceAsync();
        return Ok(records);
    }

    [HttpGet("absence-requests")]
    public async Task<IActionResult> GetAllAbsenceRequests()
    {
        var requests = await _adminRepository.GetAllAbsenceRequestsAsync();
        return Ok(requests);
    }

    [HttpPut("absence-requests/{requestId}/approve")]
    public async Task<IActionResult> ApproveAbsenceRequest(int requestId)
    {
        await _adminRepository.ApproveAbsenceRequestAsync(requestId);
        return Ok("Request approved.");
    }

    [HttpPut("absence-requests/{requestId}/reject")]
    public async Task<IActionResult> RejectAbsenceRequest(int requestId)
    {
        await _adminRepository.RejectAbsenceRequestAsync(requestId);
        return Ok("Request rejected.");
    }

    [HttpPut("attendance-record/{attendanceId}/status")]
    public async Task<IActionResult> UpdateAttendanceStatus([FromBody] AttendanceStatus attendanceStatus , int attendanceId)
    {
        await _adminRepository.UpdateAttendanceStatusAsync(attendanceStatus, attendanceId);
        return Ok("Attendance Status Updated.");
    }

    [HttpGet("reports")]
    public async Task<IActionResult> GenerateReport([FromQuery] int? workerId, [FromQuery] bool yearly, [FromQuery] int year, [FromQuery] int? month = null)
    {
        var report = await _adminRepository.GenerateReportAsync(workerId, yearly, year, month);
        return Ok(report);
    }

    [HttpPost("generate-barcode-code")]
    public async Task<IActionResult> GenerateBarcodeCode()
    {
        var (barcodeCode, qrCodeBase64) = await _adminRepository.GenerateBarcodeCodeAsync();
        return Ok(new { BarcodeCode = barcodeCode, QrCodeBase64 = qrCodeBase64 });
    }
}

// DTOs for AdminController
public class AddWorkerDto
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class AssignCheckInOutDto
{
    public int WorkerId { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime CheckOutTime { get; set; }
}
