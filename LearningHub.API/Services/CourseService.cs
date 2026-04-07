using LearningHub.API.Data;
using LearningHub.API.DTOs;
using LearningHub.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningHub.API.Services
{
    public class CourseService
    {
        private readonly AppDbContext _db;

        public CourseService(AppDbContext db)
        {
            _db = db;
        }

        // ── Get all published courses (students/browse) ────
        public async Task<List<CourseDto>> GetPublishedCoursesAsync()
        {
            return await _db.Courses
                .Where(c => c.IsPublished)
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                .Include(c => c.Modules)
                .Select(c => MapToDto(c))
                .ToListAsync();
        }

        // ── Get all courses (admin) ────────────────────────
        public async Task<List<CourseDto>> GetAllCoursesAsync()
        {
            return await _db.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                .Include(c => c.Modules)
                .Select(c => MapToDto(c))
                .ToListAsync();
        }

        // ── Get courses by instructor ──────────────────────
        public async Task<List<CourseDto>> GetInstructorCoursesAsync(int instructorId)
        {
            return await _db.Courses
                .Where(c => c.InstructorId == instructorId)
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                .Include(c => c.Modules)
                .Select(c => MapToDto(c))
                .ToListAsync();
        }

        // ── Get single course with modules + materials ─────
        public async Task<object?> GetCourseDetailAsync(int courseId, int userId, string role)
        {
            var course = await _db.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Materials)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null) return null;

            // Students can only view courses they are enrolled in
            if (role == "Student")
            {
                bool isEnrolled = course.Enrollments.Any(e => e.StudentId == userId);
                if (!isEnrolled) return null;
            }

            // Instructors can only view their own courses
            if (role == "Instructor" && course.InstructorId != userId)
                return null;

            return new
            {
                id = course.Id,
                title = course.Title,
                description = course.Description,
                isPublished = course.IsPublished,
                createdAt = course.CreatedAt,
                instructorId = course.InstructorId,
                instructorName = course.Instructor.FullName,
                modules = course.Modules.OrderBy(m => m.OrderIndex).Select(m => new
                {
                    id = m.Id,
                    title = m.Title,
                    orderIndex = m.OrderIndex,
                    courseId = m.CourseId,
                    createdAt = m.CreatedAt,
                    materials = m.Materials.OrderBy(mat => mat.OrderIndex).Select(mat => new
                    {
                        id = mat.Id,
                        title = mat.Title,
                        type = mat.Type,
                        fileUrl = mat.FileUrl,
                        orderIndex = mat.OrderIndex,
                        moduleId = mat.ModuleId,
                        createdAt = mat.CreatedAt
                    })
                })
            };
        }

        // ── Create course ──────────────────────────────────
        public async Task<CourseDto> CreateCourseAsync(CreateCourseDto dto, int instructorId)
        {
            var course = new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                InstructorId = instructorId,
                IsPublished = false
            };

            _db.Courses.Add(course);
            await _db.SaveChangesAsync();

            await _db.Entry(course).Reference(c => c.Instructor).LoadAsync();
            return MapToDto(course);
        }

        // ── Update course ──────────────────────────────────
        public async Task<CourseDto?> UpdateCourseAsync(int courseId, UpdateCourseDto dto, int instructorId, string role)
        {
            var course = await _db.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                .Include(c => c.Modules)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null) return null;

            // Instructors can only edit their own courses
            if (role == "Instructor" && course.InstructorId != instructorId)
                return null;

            course.Title = dto.Title;
            course.Description = dto.Description;
            course.IsPublished = dto.IsPublished;

            await _db.SaveChangesAsync();
            return MapToDto(course);
        }

        // ── Delete course ──────────────────────────────────
        public async Task<bool> DeleteCourseAsync(int courseId, int userId, string role)
        {
            var course = await _db.Courses.FindAsync(courseId);
            if (course == null) return false;

            if (role == "Instructor" && course.InstructorId != userId)
                return false;

            _db.Courses.Remove(course);
            await _db.SaveChangesAsync();
            return true;
        }

        // ── Map helper ─────────────────────────────────────
        private static CourseDto MapToDto(Course c) => new()
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            IsPublished = c.IsPublished,
            CreatedAt = c.CreatedAt,
            InstructorId = c.InstructorId,
            InstructorName = c.Instructor?.FullName ?? "",
            EnrollmentCount = c.Enrollments?.Count ?? 0,
            ModuleCount = c.Modules?.Count ?? 0
        };
    }
}