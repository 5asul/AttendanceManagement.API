using Microsoft.AspNetCore.Mvc;
using MyAttendanceApp.Models;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;


    public AuthController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public class CreateAdminDto
    {
        public string Name { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    // Create initial admin account
    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] CreateAdminDto dto)
    {
        var userRepo = _unitOfWork.Repository<User>();

        // Check if email already exists
        var existingUsers = await userRepo.FindAsync(u => u.PhoneNumber == dto.PhoneNumber);
        if (existingUsers.Any())
            return BadRequest("Phone Number already exists.");

        var admin = new User
        {
            Name = dto.Name,
            PhoneNumber = dto.PhoneNumber,
            Password = dto.Password, // In production, hash this
            Role = UserRole.admin,
            CreatedAt = DateTime.UtcNow
        };

        await userRepo.AddAsync(admin);
        await _unitOfWork.SaveChangesAsync();

        return Ok("Admin account created successfully.");
    }
}
