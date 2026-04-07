using LearningHub.API.DTOs;
using LearningHub.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearningHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly CourseService _courseService;

        public CoursesController(CourseService courseService)
        {
            _courseService = courseService;
        }

        // GET api/courses/published — public browse
        [HttpGet("published")]
        public async Task<IActionResult> GetPublished()
        {
            var courses = await _courseService.GetPublishedCoursesAsync();
            return Ok(courses);
        }

        // GET api/courses — admin sees all
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return Ok(courses);
        }

        // GET api/courses/my — instructor sees own courses
        [Authorize(Roles = "Instructor")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyCourses()
        {
            int userId = GetUserId();
            var courses = await _courseService.GetInstructorCoursesAsync(userId);
            return Ok(courses);
        }

        // GET api/courses/{id} — detail with modules & materials
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            int userId = GetUserId();
            string role = GetRole();
            var result = await _courseService.GetCourseDetailAsync(id, userId, role);
            if (result == null) return NotFound(new { message = "Course not found or access denied." });
            return Ok(result);
        }

        // POST api/courses — instructor creates
        [Authorize(Roles = "Instructor")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCourseDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest(new { message = "Title is required." });

            int userId = GetUserId();
            var course = await _courseService.CreateCourseAsync(dto, userId);
            return CreatedAtAction(nameof(GetDetail), new { id = course.Id }, course);
        }

        // PUT api/courses/{id} — instructor or admin updates
        [Authorize(Roles = "Instructor,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCourseDto dto)
        {
            int userId = GetUserId();
            string role = GetRole();
            var result = await _courseService.UpdateCourseAsync(id, dto, userId, role);
            if (result == null) return NotFound(new { message = "Course not found or access denied." });
            return Ok(result);
        }

        // DELETE api/courses/{id}
        [Authorize(Roles = "Instructor,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int userId = GetUserId();
            string role = GetRole();
            bool deleted = await _courseService.DeleteCourseAsync(id, userId, role);
            if (!deleted) return NotFound(new { message = "Course not found or access denied." });
            return NoContent();
        }

        // ── Helpers ────────────────────────────────────────
        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private string GetRole() => User.FindFirstValue(ClaimTypes.Role)!;
    }
}