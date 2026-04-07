using LearningHub.API.DTOs;
using LearningHub.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearningHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProgressController : ControllerBase
    {
        private readonly ProgressService _progressService;

        public ProgressController(ProgressService progressService)
        {
            _progressService = progressService;
        }

        // POST api/progress — mark material complete
        [Authorize(Roles = "Student")]
        [HttpPost]
        public async Task<IActionResult> MarkComplete([FromBody] MarkCompleteDto dto)
        {
            int studentId = GetUserId();
            var (success, message, data) =
                await _progressService.MarkCompleteAsync(studentId, dto.CourseId, dto.MaterialId);

            if (!success) return BadRequest(new { message });
            return Ok(new { message, data });
        }

        // DELETE api/progress/{courseId}/{materialId} — unmark complete
        [Authorize(Roles = "Student")]
        [HttpDelete("{courseId}/{materialId}")]
        public async Task<IActionResult> UnmarkComplete(int courseId, int materialId)
        {
            int studentId = GetUserId();
            var (success, message) =
                await _progressService.UnmarkCompleteAsync(studentId, courseId, materialId);

            if (!success) return BadRequest(new { message });
            return Ok(new { message });
        }

        // GET api/progress/course/{courseId} — get progress for one course
        [Authorize(Roles = "Student")]
        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetCourseProgress(int courseId)
        {
            int studentId = GetUserId();
            var progress = await _progressService.GetCourseProgressAsync(studentId, courseId);
            if (progress == null)
                return NotFound(new { message = "Enrollment not found." });
            return Ok(progress.CompletedItems);
        }

        // GET api/progress/my — all progress across all courses
        [Authorize(Roles = "Student")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyProgress()
        {
            int studentId = GetUserId();
            var progress = await _progressService.GetAllStudentProgressAsync(studentId);
            return Ok(progress);
        }

        // GET api/progress/course/{courseId}/students — instructor view
        [Authorize(Roles = "Instructor")]
        [HttpGet("course/{courseId}/students")]
        public async Task<IActionResult> GetStudentProgress(int courseId)
        {
            int instructorId = GetUserId();
            var progress = await _progressService.GetStudentProgressByCourseAsync(courseId, instructorId);
            return Ok(progress);
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}