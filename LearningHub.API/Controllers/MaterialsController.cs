using LearningHub.API.DTOs;
using LearningHub.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearningHub.API.Controllers
{
    [ApiController]
    [Route("api/modules/{moduleId}/materials")]
    public class MaterialsController : ControllerBase
    {
        private readonly MaterialService _materialService;

        public MaterialsController(MaterialService materialService)
        {
            _materialService = materialService;
        }

        // POST api/modules/{moduleId}/materials
        [Authorize(Roles = "Instructor")]
        [HttpPost]
        public async Task<IActionResult> Create(
            int moduleId,
            [FromForm] CreateMaterialDto dto,
            IFormFile? file)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest(new { message = "Title is required." });

            int userId = GetUserId();
            var material = await _materialService.CreateMaterialAsync(moduleId, dto, file, userId);
            if (material == null) return NotFound(new { message = "Module not found or access denied." });
            return Ok(material);
        }

        // PUT api/modules/{moduleId}/materials/{materialId}
        [Authorize(Roles = "Instructor")]
        [HttpPut("{materialId}")]
        public async Task<IActionResult> Update(
            int moduleId,
            int materialId,
            [FromForm] UpdateMaterialDto dto,
            IFormFile? file)
        {
            int userId = GetUserId();
            var material = await _materialService.UpdateMaterialAsync(materialId, dto, file, userId);
            if (material == null) return NotFound(new { message = "Material not found or access denied." });
            return Ok(material);
        }

        // DELETE api/modules/{moduleId}/materials/{materialId}
        [Authorize(Roles = "Instructor")]
        [HttpDelete("{materialId}")]
        public async Task<IActionResult> Delete(int moduleId, int materialId)
        {
            int userId = GetUserId();
            bool deleted = await _materialService.DeleteMaterialAsync(materialId, userId);
            if (!deleted) return NotFound(new { message = "Material not found or access denied." });
            return NoContent();
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}