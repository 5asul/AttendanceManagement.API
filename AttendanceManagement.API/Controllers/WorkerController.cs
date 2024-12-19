using Microsoft.AspNetCore.Mvc;
using MyAttendanceApp.Models;



[ApiController]
[Route("api/[controller]")]
// [Authorize(Roles = "worker")] // Ideally you'd have authorization here
public class WorkerController : ControllerBase
{
   
    private readonly IUnitOfWork _unitOfWork;

    public WorkerController( IUnitOfWork unitOfWork)
    {
        
        _unitOfWork = unitOfWork;
    }

    [HttpPost("check-in")]
    public async Task<IActionResult> CheckIn([FromBody] CheckInDto dto)
    {
        await _unitOfWork.WorkerRepository.CheckInAsync(dto.WorkerId, dto.BarcodeValue);
        return Ok("Checked in successfully.");
    }

    [HttpPost("check-out")]
    public async Task<IActionResult> CheckOut([FromBody] CheckOutDto dto)
    {
        await _unitOfWork.WorkerRepository.CheckOutAsync(dto.WorkerId, dto.BarcodeValue);
        return Ok("Checked out successfully.");
    }

    [HttpPost("absence-request")]
    public async Task<IActionResult> CreateAbsenceRequest([FromBody] CreateAbsenceRequestDto dto)
    {
        var request = await _unitOfWork.WorkerRepository.CreateAbsenceRequestAsync(dto.WorkerId, dto.Reason, dto.StartDate,dto.EndDate ,dto.Type);
        return Ok(request);
    }
}

// DTOs for WorkerController
public class CheckInDto
{
    public int WorkerId { get; set; }
    public string BarcodeValue { get; set; } = null!;
}

public class CheckOutDto
{
    public int WorkerId { get; set; }
    public string BarcodeValue { get; set; } = null!;
}

public class CreateAbsenceRequestDto
{
    public int WorkerId { get; set; }
    public string Reason { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public AbsenceTypes Type { get; set; } = AbsenceTypes.Absent; // "leave", "absent", "late"
}
