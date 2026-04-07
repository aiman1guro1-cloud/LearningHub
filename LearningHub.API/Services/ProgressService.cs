using LearningHub.API.Data;
using LearningHub.API.DTOs;
using LearningHub.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningHub.API.Services
{
    public class ProgressService
    {
        private readonly AppDbContext _db;

        public ProgressService(AppDbContext db)
        {
            _db = db;
        }

        // ── Mark a material as complete ────────────────────
        public async Task<(bool Success, string Message, ProgressDto? Data)> MarkCompleteAsync(
            int studentId, int courseId, int materialId)
        {
            // Verify student is enrolled
            var enrollment = await _db.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
                return (false, "You are not enrolled in this course.", null);

            // Verify material belongs to this course
            var material = await _db.Materials
                .Include(m => m.Module)
                .FirstOrDefaultAsync(m => m.Id == materialId && m.Module.CourseId == courseId);

            if (material == null)
                return (false, "Material not found in this course.", null);

            // Check if already marked complete
            bool alreadyDone = await _db.Progresses
                .AnyAsync(p => p.EnrollmentId == enrollment.Id && p.MaterialId == materialId);

            if (alreadyDone)
                return (false, "Material already marked as complete.", null);

            var progress = new Progress
            {
                EnrollmentId = enrollment.Id,
                MaterialId = materialId,
                CompletedAt = DateTime.UtcNow
            };

            _db.Progresses.Add(progress);
            await _db.SaveChangesAsync();

            return (true, "Material marked as complete.", new ProgressDto
            {
                Id = progress.Id,
                EnrollmentId = progress.EnrollmentId,
                MaterialId = progress.MaterialId,
                MaterialTitle = material.Title,
                CompletedAt = progress.CompletedAt
            });
        }

        // ── Unmark a material as complete ──────────────────
        public async Task<(bool Success, string Message)> UnmarkCompleteAsync(
            int studentId, int courseId, int materialId)
        {
            var enrollment = await _db.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
                return (false, "Enrollment not found.");

            var progress = await _db.Progresses
                .FirstOrDefaultAsync(p => p.EnrollmentId == enrollment.Id && p.MaterialId == materialId);

            if (progress == null)
                return (false, "Progress record not found.");

            _db.Progresses.Remove(progress);
            await _db.SaveChangesAsync();
            return (true, "Material unmarked.");
        }

        // ── Get progress for a student in a course ─────────
        public async Task<CourseProgressDto?> GetCourseProgressAsync(int studentId, int courseId)
        {
            var enrollment = await _db.Enrollments
                .Include(e => e.Course)
                    .ThenInclude(c => c.Modules)
                        .ThenInclude(m => m.Materials)
                .Include(e => e.Progresses)
                    .ThenInclude(p => p.Material)
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null) return null;

            int total = enrollment.Course.Modules.SelectMany(m => m.Materials).Count();
            int completed = enrollment.Progresses.Count;

            return new CourseProgressDto
            {
                CourseId = courseId,
                CourseTitle = enrollment.Course.Title,
                TotalMaterials = total,
                CompletedMaterials = completed,
                ProgressPercent = total > 0
                    ? Math.Round((double)completed / total * 100, 1)
                    : 0,
                CompletedItems = enrollment.Progresses.Select(p => new ProgressDto
                {
                    Id = p.Id,
                    EnrollmentId = p.EnrollmentId,
                    MaterialId = p.MaterialId,
                    MaterialTitle = p.Material.Title,
                    CompletedAt = p.CompletedAt
                }).ToList()
            };
        }

        // ── Get all progress across all courses (student) ──
        public async Task<List<CourseProgressDto>> GetAllStudentProgressAsync(int studentId)
        {
            var enrollments = await _db.Enrollments
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Modules)
                        .ThenInclude(m => m.Materials)
                .Include(e => e.Progresses)
                .ToListAsync();

            return enrollments.Select(e =>
            {
                int total = e.Course.Modules.SelectMany(m => m.Materials).Count();
                int completed = e.Progresses.Count;
                return new CourseProgressDto
                {
                    CourseId = e.CourseId,
                    CourseTitle = e.Course.Title,
                    TotalMaterials = total,
                    CompletedMaterials = completed,
                    ProgressPercent = total > 0
                        ? Math.Round((double)completed / total * 100, 1)
                        : 0
                };
            }).ToList();
        }

        // ── Get progress for all students in a course ──────
        // (used by instructor)
        public async Task<List<object>> GetStudentProgressByCourseAsync(int courseId, int instructorId)
        {
            var course = await _db.Courses
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Materials)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null || course.InstructorId != instructorId)
                return new List<object>();

            int totalMaterials = course.Modules.SelectMany(m => m.Materials).Count();

            var enrollments = await _db.Enrollments
                .Where(e => e.CourseId == courseId)
                .Include(e => e.Student)
                .Include(e => e.Progresses)
                .ToListAsync();

            return enrollments.Select(e =>
            {
                int completed = e.Progresses.Count;
                return (object)new
                {
                    studentId = e.StudentId,
                    studentName = e.Student.FullName,
                    studentEmail = e.Student.Email,
                    enrolledAt = e.EnrolledAt,
                    completedMaterials = completed,
                    totalMaterials = totalMaterials,
                    progressPercent = totalMaterials > 0
                        ? Math.Round((double)completed / totalMaterials * 100, 1)
                        : 0
                };
            }).ToList();
        }
    }
}