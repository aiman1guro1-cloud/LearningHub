using LearningHub.API.Data;
using LearningHub.API.DTOs;
using LearningHub.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningHub.API.Services
{
    public class AnnouncementService
    {
        private readonly AppDbContext _db;

        public AnnouncementService(AppDbContext db)
        {
            _db = db;
        }

        // ── Get announcements for a course ─────────────────
        public async Task<List<AnnouncementDto>> GetByCourseAsync(int courseId)
        {
            return await _db.Announcements
                .Where(a => a.CourseId == courseId)
                .Include(a => a.Instructor)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new AnnouncementDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Content = a.Content,
                    CourseId = a.CourseId,
                    InstructorId = a.InstructorId,
                    InstructorName = a.Instructor.FullName,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();
        }

        // ── Create an announcement ─────────────────────────
        public async Task<(bool Success, string Message, AnnouncementDto? Data)>
            CreateAsync(int courseId, int instructorId, CreateAnnouncementDto dto)
        {
            // Verify course ownership
            var course = await _db.Courses.FindAsync(courseId);
            if (course == null || course.InstructorId != instructorId)
                return (false, "Course not found or access denied.", null);

            var announcement = new Announcement
            {
                Title = dto.Title,
                Content = dto.Content,
                CourseId = courseId,
                InstructorId = instructorId,
                CreatedAt = DateTime.UtcNow
            };

            _db.Announcements.Add(announcement);
            await _db.SaveChangesAsync();

            var instructor = await _db.Users.FindAsync(instructorId);

            return (true, "Announcement created.", new AnnouncementDto
            {
                Id = announcement.Id,
                Title = announcement.Title,
                Content = announcement.Content,
                CourseId = announcement.CourseId,
                InstructorId = announcement.InstructorId,
                InstructorName = instructor?.FullName ?? "",
                CreatedAt = announcement.CreatedAt
            });
        }

        // ── Update an announcement ─────────────────────────
        public async Task<(bool Success, string Message, AnnouncementDto? Data)>
            UpdateAsync(int announcementId, int instructorId, UpdateAnnouncementDto dto)
        {
            var announcement = await _db.Announcements
                .Include(a => a.Instructor)
                .FirstOrDefaultAsync(a => a.Id == announcementId);

            if (announcement == null)
                return (false, "Announcement not found.", null);

            if (announcement.InstructorId != instructorId)
                return (false, "You can only edit your own announcements.", null);

            announcement.Title = dto.Title;
            announcement.Content = dto.Content;
            await _db.SaveChangesAsync();

            return (true, "Announcement updated.", new AnnouncementDto
            {
                Id = announcement.Id,
                Title = announcement.Title,
                Content = announcement.Content,
                CourseId = announcement.CourseId,
                InstructorId = announcement.InstructorId,
                InstructorName = announcement.Instructor.FullName,
                CreatedAt = announcement.CreatedAt
            });
        }

        // ── Delete an announcement ─────────────────────────
        public async Task<(bool Success, string Message)>
            DeleteAsync(int announcementId, int instructorId, string role)
        {
            var announcement = await _db.Announcements.FindAsync(announcementId);
            if (announcement == null) return (false, "Announcement not found.");

            if (announcement.InstructorId != instructorId && role != "Admin")
                return (false, "You cannot delete this announcement.");

            _db.Announcements.Remove(announcement);
            await _db.SaveChangesAsync();
            return (true, "Announcement deleted.");
        }
    }
}