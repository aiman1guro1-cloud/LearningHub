using LearningHub.API.DTOs;
using LearningHub.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearningHub.API.Controllers
{
    [ApiController]
    [Route("api/courses/{courseId}/announcements")]
    public class AnnouncementsController : ControllerBase
    {
        private readonly AnnouncementService _announcementService;

        public AnnouncementsController(AnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        // GET api/courses/{courseId}/announcements
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll(int courseId)
        {
            var announcements = await _announcementService.GetByCourseAsync(courseId);
            return Ok(announcements);
        }

        // POST api/courses/{courseId}/announcements
        [Authorize(Roles = "Instructor")]
        [HttpPost]
        public async Task<IActionResult> Create(
            int courseId, [FromBody] CreateAnnouncementDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Content))
                return BadRequest(new { message = "Title and content are required." });

            int instructorId = GetUserId();
            var (success, message, data) =
                await _announcementService.CreateAsync(courseId, instructorId, dto);

            if (!success) return BadRequest(new { message });
            return Ok(data);
        }

        // PUT api/courses/{courseId}/announcements/{id}
        [Authorize(Roles = "Instructor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int courseId, int id, [FromBody] UpdateAnnouncementDto dto)
        {
            int instructorId = GetUserId();
            var (success, message, data) =
                await _announcementService.UpdateAsync(id, instructorId, dto);

            if (!success) return BadRequest(new { message });
            return Ok(data);
        }

        // DELETE api/courses/{courseId}/announcements/{id}
        [Authorize(Roles = "Instructor,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int courseId, int id)
        {
            int userId = GetUserId();
            string role = GetRole();

            var (success, message) =
                await _announcementService.DeleteAsync(id, userId, role);

            if (!success) return BadRequest(new { message });
            return NoContent();
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private string GetRole() => User.FindFirstValue(ClaimTypes.Role)!;
    }
}