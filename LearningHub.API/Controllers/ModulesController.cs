using LearningHub.API.DTOs;
using LearningHub.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearningHub.API.Controllers
{
    [ApiController]
    [Route("api/courses/{courseId}/modules")]
    public class ModulesController : ControllerBase
    {
        private readonly ModuleService _moduleService;

        public ModulesController(ModuleService moduleService)
        {
            _moduleService = moduleService;
        }

        // GET api/courses/{courseId}/modules
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll(int courseId)
        {
            var modules = await _moduleService.GetModulesByCourseAsync(courseId);
            return Ok(modules);
        }

        // POST api/courses/{courseId}/modules
        [Authorize(Roles = "Instructor")]
        [HttpPost]
        public async Task<IActionResult> Create(int courseId, [FromBody] CreateModuleDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest(new { message = "Title is required." });

            int userId = GetUserId();
            var module = await _moduleService.CreateModuleAsync(courseId, dto, userId);
            if (module == null) return NotFound(new { message = "Course not found or access denied." });
            return Ok(module);
        }

        // PUT api/courses/{courseId}/modules/{moduleId}
        [Authorize(Roles = "Instructor")]
        [HttpPut("{moduleId}")]
        public async Task<IActionResult> Update(int courseId, int moduleId, [FromBody] UpdateModuleDto dto)
        {
            int userId = GetUserId();
            var module = await _moduleService.UpdateModuleAsync(moduleId, dto, userId);
            if (module == null) return NotFound(new { message = "Module not found or access denied." });
            return Ok(module);
        }

        // DELETE api/courses/{courseId}/modules/{moduleId}
        [Authorize(Roles = "Instructor")]
        [HttpDelete("{moduleId}")]
        public async Task<IActionResult> Delete(int courseId, int moduleId)
        {
            int userId = GetUserId();
            bool deleted = await _moduleService.DeleteModuleAsync(moduleId, userId);
            if (!deleted) return NotFound(new { message = "Module not found or access denied." });
            return NoContent();
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}