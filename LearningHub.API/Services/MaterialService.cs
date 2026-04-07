using LearningHub.API.Data;
using LearningHub.API.DTOs;
using LearningHub.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningHub.API.Services
{
    public class MaterialService
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public MaterialService(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<MaterialDto?> CreateMaterialAsync(
            int moduleId,
            CreateMaterialDto dto,
            IFormFile? file,
            int instructorId)
        {
            // Verify ownership
            var module = await _db.Modules
                .Include(m => m.Course)
                .FirstOrDefaultAsync(m => m.Id == moduleId);

            if (module == null || module.Course.InstructorId != instructorId)
                return null;

            string fileUrl = string.Empty;

            if (file != null && file.Length > 0)
            {
                fileUrl = await SaveFileAsync(file);
            }

            var material = new Material
            {
                Title = dto.Title,
                Type = dto.Type,
                FileUrl = fileUrl,
                OrderIndex = dto.OrderIndex,
                ModuleId = moduleId
            };

            _db.Materials.Add(material);
            await _db.SaveChangesAsync();

            return MapToDto(material);
        }

        public async Task<MaterialDto?> UpdateMaterialAsync(
            int materialId,
            UpdateMaterialDto dto,
            IFormFile? file,
            int instructorId)
        {
            var material = await _db.Materials
                .Include(m => m.Module)
                    .ThenInclude(mod => mod.Course)
                .FirstOrDefaultAsync(m => m.Id == materialId);

            if (material == null || material.Module.Course.InstructorId != instructorId)
                return null;

            material.Title = dto.Title;
            material.OrderIndex = dto.OrderIndex;

            if (file != null && file.Length > 0)
            {
                // Delete old file if it exists
                DeleteFile(material.FileUrl);
                material.FileUrl = await SaveFileAsync(file);
            }

            await _db.SaveChangesAsync();
            return MapToDto(material);
        }

        public async Task<bool> DeleteMaterialAsync(int materialId, int instructorId)
        {
            var material = await _db.Materials
                .Include(m => m.Module)
                    .ThenInclude(mod => mod.Course)
                .FirstOrDefaultAsync(m => m.Id == materialId);

            if (material == null || material.Module.Course.InstructorId != instructorId)
                return false;

            DeleteFile(material.FileUrl);
            _db.Materials.Remove(material);
            await _db.SaveChangesAsync();
            return true;
        }

        // ── File helpers ───────────────────────────────────
        private async Task<string> SaveFileAsync(IFormFile file)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/{uniqueName}";
        }

        private void DeleteFile(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) return;

            var fullPath = Path.Combine(
                _env.WebRootPath ?? "wwwroot",
                fileUrl.TrimStart('/'));

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        private static MaterialDto MapToDto(Material mat) => new()
        {
            Id = mat.Id,
            Title = mat.Title,
            Type = mat.Type,
            FileUrl = mat.FileUrl,
            OrderIndex = mat.OrderIndex,
            ModuleId = mat.ModuleId,
            CreatedAt = mat.CreatedAt
        };
    }
}