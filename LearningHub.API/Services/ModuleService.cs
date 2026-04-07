using LearningHub.API.Data;
using LearningHub.API.DTOs;
using LearningHub.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningHub.API.Services
{
    public class ModuleService
    {
        private readonly AppDbContext _db;

        public ModuleService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<ModuleDto>> GetModulesByCourseAsync(int courseId)
        {
            return await _db.Modules
                .Where(m => m.CourseId == courseId)
                .Include(m => m.Materials)
                .OrderBy(m => m.OrderIndex)
                .Select(m => new ModuleDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    OrderIndex = m.OrderIndex,
                    CourseId = m.CourseId,
                    CreatedAt = m.CreatedAt,
                    Materials = m.Materials
                        .OrderBy(mat => mat.OrderIndex)
                        .Select(mat => new MaterialDto
                        {
                            Id = mat.Id,
                            Title = mat.Title,
                            Type = mat.Type,
                            FileUrl = mat.FileUrl,
                            OrderIndex = mat.OrderIndex,
                            ModuleId = mat.ModuleId,
                            CreatedAt = mat.CreatedAt
                        }).ToList()
                }).ToListAsync();
        }

        public async Task<ModuleDto?> CreateModuleAsync(int courseId, CreateModuleDto dto, int instructorId)
        {
            // Verify the course belongs to this instructor
            var course = await _db.Courses.FindAsync(courseId);
            if (course == null || course.InstructorId != instructorId) return null;

            var module = new Module
            {
                Title = dto.Title,
                OrderIndex = dto.OrderIndex,
                CourseId = courseId
            };

            _db.Modules.Add(module);
            await _db.SaveChangesAsync();

            return new ModuleDto
            {
                Id = module.Id,
                Title = module.Title,
                OrderIndex = module.OrderIndex,
                CourseId = module.CourseId,
                CreatedAt = module.CreatedAt,
                Materials = new List<MaterialDto>()
            };
        }

        public async Task<ModuleDto?> UpdateModuleAsync(int moduleId, UpdateModuleDto dto, int instructorId)
        {
            var module = await _db.Modules
                .Include(m => m.Course)
                .Include(m => m.Materials)
                .FirstOrDefaultAsync(m => m.Id == moduleId);

            if (module == null || module.Course.InstructorId != instructorId) return null;

            module.Title = dto.Title;
            module.OrderIndex = dto.OrderIndex;
            await _db.SaveChangesAsync();

            return new ModuleDto
            {
                Id = module.Id,
                Title = module.Title,
                OrderIndex = module.OrderIndex,
                CourseId = module.CourseId,
                CreatedAt = module.CreatedAt,
                Materials = module.Materials.Select(mat => new MaterialDto
                {
                    Id = mat.Id,
                    Title = mat.Title,
                    Type = mat.Type,
                    FileUrl = mat.FileUrl,
                    OrderIndex = mat.OrderIndex,
                    ModuleId = mat.ModuleId,
                    CreatedAt = mat.CreatedAt
                }).ToList()
            };
        }

        public async Task<bool> DeleteModuleAsync(int moduleId, int instructorId)
        {
            var module = await _db.Modules
                .Include(m => m.Course)
                .FirstOrDefaultAsync(m => m.Id == moduleId);

            if (module == null || module.Course.InstructorId != instructorId) return false;

            _db.Modules.Remove(module);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}