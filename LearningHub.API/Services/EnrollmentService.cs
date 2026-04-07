using LearningHub.API.Data;
using LearningHub.API.DTOs;
using LearningHub.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningHub.API.Services
{
    public class EnrollmentService
    {
        private readonly AppDbContext _db;

        public EnrollmentService(AppDbContext db)
        {
            _db = db;
        }

        // ── Enroll a student in a course ───────────────────
        public async Task<(bool Success, string Message, EnrollmentDto? Data)> EnrollAsync(int studentId, int courseId)
        {
            // Check course exists and is published
            var course = await _db.Courses.FindAsync(courseId);
            if (course == null || !course.IsPublished)
                return (false, "Course not found or not available.", null);

            // Check not already enrolled
            bool alreadyEnrolled = await _db.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);
            if (alreadyEnrolled)
                return (false, "You are already enrolled in this course.", null);

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrolledAt = DateTime.UtcNow
            };

            _db.Enrollments.Add(enrollment);
            await _db.SaveChangesAsync();

            return (true, "Enrolled successfully.", new EnrollmentDto
            {
                Id = enrollment.Id,
                StudentId = studentId,
                CourseId = courseId,
                CourseTitle = course.Title,
                EnrolledAt = enrollment.EnrolledAt,
                CompletedMaterials = 0,
                TotalMaterials = 0,
                ProgressPercent = 0
            });
        }

        // ── Unenroll a student ─────────────────────────────
        public async Task<(bool Success, string Message)> UnenrollAsync(int studentId, int courseId)
        {
            var enrollment = await _db.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
                return (false, "Enrollment not found.");

            _db.Enrollments.Remove(enrollment);
            await _db.SaveChangesAsync();
            return (true, "Unenrolled successfully.");
        }

        // ── Get all enrollments for a student ──────────────
        public async Task<List<EnrollmentDto>> GetStudentEnrollmentsAsync(int studentId)
        {
            var enrollments = await _db.Enrollments
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Modules)
                        .ThenInclude(m => m.Materials)
                .Include(e => e.Progresses)
                .OrderByDescending(e => e.EnrolledAt)
                .ToListAsync();

            return enrollments.Select(e =>
            {
                int total = e.Course.Modules.SelectMany(m => m.Materials).Count();
                int completed = e.Progresses.Count;
                return new EnrollmentDto
                {
                    Id = e.Id,
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    CourseTitle = e.Course.Title,
                    EnrolledAt = e.EnrolledAt,
                    CompletedMaterials = completed,
                    TotalMaterials = total,
                    ProgressPercent = total > 0 ? Math.Round((double)completed / total * 100, 1) : 0
                };
            }).ToList();
        }

        // ── Get all enrollments for a course (instructor) ──
        public async Task<List<EnrollmentDto>> GetCourseEnrollmentsAsync(int courseId, int instructorId)
        {
            var course = await _db.Courses.FindAsync(courseId);
            if (course == null || course.InstructorId != instructorId)
                return new List<EnrollmentDto>();

            return await _db.Enrollments
                .Where(e => e.CourseId == courseId)
                .Include(e => e.Student)
                .Include(e => e.Progresses)
                .Select(e => new EnrollmentDto
                {
                    Id = e.Id,
                    StudentId = e.StudentId,
                    StudentName = e.Student.FullName,
                    CourseId = e.CourseId,
                    CourseTitle = course.Title,
                    EnrolledAt = e.EnrolledAt
                })
                .ToListAsync();
        }

        // ── Get all enrollments (admin) ────────────────────
        public async Task<List<EnrollmentDto>> GetAllEnrollmentsAsync()
        {
            return await _db.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Select(e => new EnrollmentDto
                {
                    Id = e.Id,
                    StudentId = e.StudentId,
                    StudentName = e.Student.FullName,
                    CourseId = e.CourseId,
                    CourseTitle = e.Course.Title,
                    EnrolledAt = e.EnrolledAt
                })
                .ToListAsync();
        }

        // ── Check if student is enrolled ───────────────────
        public async Task<bool> IsEnrolledAsync(int studentId, int courseId)
        {
            return await _db.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);
        }
    }
}