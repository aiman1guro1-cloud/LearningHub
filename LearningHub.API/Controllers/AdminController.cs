using LearningHub.API.DTOs;
using LearningHub.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearningHub.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;
        private readonly CourseService _courseService;

        public AdminController(AdminService adminService, CourseService courseService)
        {
            _adminService = adminService;
            _courseService = courseService;
        }

        // ── Stats ──────────────────────────────────────────
        // GET api/admin/stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _adminService.GetStatsAsync();
            return Ok(stats);
        }

        // ── Users ──────────────────────────────────────────
        // GET api/admin/users
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET api/admin/users/{id}
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _adminService.GetUserByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found." });
            return Ok(user);
        }

        // POST api/admin/users
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { message = "All fields are required." });

            if (dto.Password.Length < 6)
                return BadRequest(new { message = "Password must be at least 6 characters." });

            var (success, message, data) = await _adminService.CreateUserAsync(dto);
            if (!success) return BadRequest(new { message });
            return Ok(data);
        }

        // PUT api/admin/users/{id}
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName) ||
                string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { message = "Name and email are required." });

            var (success, message, data) = await _adminService.UpdateUserAsync(id, dto);
            if (!success) return BadRequest(new { message });
            return Ok(data);
        }

        // DELETE api/admin/users/{id}
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            int adminId = GetUserId();
            var (success, message) = await _adminService.DeleteUserAsync(id, adminId);
            if (!success) return BadRequest(new { message });
            return NoContent();
        }

        // POST api/admin/users/{id}/reset-password
        [HttpPost("users/{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(
            int id, [FromBody] ResetPasswordRequest request)
        {
            var (success, message) =
                await _adminService.ResetPasswordAsync(id, request.NewPassword);
            if (!success) return BadRequest(new { message });
            return Ok(new { message });
        }

        // ── Courses ────────────────────────────────────────
        // GET api/admin/courses
        [HttpGet("courses")]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _adminService.GetAllCoursesAsync();
            return Ok(courses);
        }

        // DELETE api/admin/courses/{id}
        [HttpDelete("courses/{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            int adminId = GetUserId();
            bool deleted = await _courseService.DeleteCourseAsync(id, adminId, "Admin");
            if (!deleted) return NotFound(new { message = "Course not found." });
            return NoContent();
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    // ── Request model for password reset ──────────────────
    public class ResetPasswordRequest
    {
        public string NewPassword { get; set; } = string.Empty;
    }
}