using LearningHub.API.DTOs;
using LearningHub.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearningHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly EnrollmentService _enrollmentService;

        public EnrollmentsController(EnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        // POST api/enrollments — student enrolls
        [Authorize(Roles = "Student")]
        [HttpPost]
        public async Task<IActionResult> Enroll([FromBody] EnrollRequestDto dto)
        {
            int studentId = GetUserId();
            var (success, message, data) = await _enrollmentService.EnrollAsync(studentId, dto.CourseId);

            if (!success) return BadRequest(new { message });
            return Ok(new { message, data });
        }

        // DELETE api/enrollments/{courseId} — student unenrolls
        [Authorize(Roles = "Student")]
        [HttpDelete("{courseId}")]
        public async Task<IActionResult> Unenroll(int courseId)
        {
            int studentId = GetUserId();
            var (success, message) = await _enrollmentService.UnenrollAsync(studentId, courseId);

            if (!success) return BadRequest(new { message });
            return Ok(new { message });
        }

        // GET api/enrollments/my — student sees their enrollments
        [Authorize(Roles = "Student")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyEnrollments()
        {
            int studentId = GetUserId();
            var enrollments = await _enrollmentService.GetStudentEnrollmentsAsync(studentId);
            return Ok(enrollments);
        }

        // GET api/enrollments/course/{courseId} — instructor sees their course enrollments
        [Authorize(Roles = "Instructor")]
        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetCourseEnrollments(int courseId)
        {
            int instructorId = GetUserId();
            var enrollments = await _enrollmentService.GetCourseEnrollmentsAsync(courseId, instructorId);
            return Ok(enrollments);
        }

        // GET api/enrollments — admin sees all
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var enrollments = await _enrollmentService.GetAllEnrollmentsAsync();
            return Ok(enrollments);
        }

        // GET api/enrollments/check/{courseId} — check enrollment status
        [Authorize(Roles = "Student")]
        [HttpGet("check/{courseId}")]
        public async Task<IActionResult> CheckEnrollment(int courseId)
        {
            int studentId = GetUserId();
            bool isEnrolled = await _enrollmentService.IsEnrolledAsync(studentId, courseId);
            return Ok(new { isEnrolled });
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}