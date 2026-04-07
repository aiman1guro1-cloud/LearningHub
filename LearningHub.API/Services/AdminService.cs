using LearningHub.API.Data;
using LearningHub.API.DTOs;
using LearningHub.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningHub.API.Services
{
    public class AdminService
    {
        private readonly AppDbContext _db;

        public AdminService(AppDbContext db)
        {
            _db = db;
        }

        // ── System stats ───────────────────────────────────
        public async Task<SystemStatsDto> GetStatsAsync()
        {
            var users = await _db.Users.ToListAsync();
            var courses = await _db.Courses.ToListAsync();
            var enrollments = await _db.Enrollments.CountAsync();
            var materials = await _db.Materials.CountAsync();
            var posts = await _db.DiscussionPosts.CountAsync();

            return new SystemStatsDto
            {
                TotalUsers = users.Count,
                TotalStudents = users.Count(u => u.Role == "Student"),
                TotalInstructors = users.Count(u => u.Role == "Instructor"),
                TotalAdmins = users.Count(u => u.Role == "Admin"),
                TotalCourses = courses.Count,
                PublishedCourses = courses.Count(c => c.IsPublished),
                DraftCourses = courses.Count(c => !c.IsPublished),
                TotalEnrollments = enrollments,
                TotalMaterials = materials,
                TotalDiscussionPosts = posts
            };
        }

        // ── Get all users ──────────────────────────────────
        public async Task<List<AdminUserDto>> GetAllUsersAsync()
        {
            return await _db.Users
                .Include(u => u.Courses)
                .Include(u => u.Enrollments)
                .OrderBy(u => u.Role)
                .ThenBy(u => u.FullName)
                .Select(u => new AdminUserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    CourseCount = u.Courses.Count,
                    EnrollmentCount = u.Enrollments.Count
                })
                .ToListAsync();
        }

        // ── Get user by ID ─────────────────────────────────
        public async Task<AdminUserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _db.Users
                .Include(u => u.Courses)
                .Include(u => u.Enrollments)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;

            return new AdminUserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                CourseCount = user.Courses.Count,
                EnrollmentCount = user.Enrollments.Count
            };
        }

        // ── Create user (admin only) ───────────────────────
        public async Task<(bool Success, string Message, AdminUserDto? Data)>
            CreateUserAsync(CreateUserDto dto)
        {
            bool emailExists = await _db.Users.AnyAsync(u => u.Email == dto.Email);
            if (emailExists)
                return (false, "An account with this email already exists.", null);

            var allowedRoles = new[] { "Admin", "Instructor", "Student" };
            if (!allowedRoles.Contains(dto.Role))
                return (false, "Invalid role.", null);

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return (true, "User created.", new AdminUserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            });
        }

        // ── Update user ────────────────────────────────────
        public async Task<(bool Success, string Message, AdminUserDto? Data)>
            UpdateUserAsync(int userId, UpdateUserDto dto)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null)
                return (false, "User not found.", null);

            // Check email not taken by another user
            bool emailTaken = await _db.Users
                .AnyAsync(u => u.Email == dto.Email && u.Id != userId);
            if (emailTaken)
                return (false, "Email already in use by another account.", null);

            var allowedRoles = new[] { "Admin", "Instructor", "Student" };
            if (!allowedRoles.Contains(dto.Role))
                return (false, "Invalid role.", null);

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.Role = dto.Role;
            await _db.SaveChangesAsync();

            return (true, "User updated.", new AdminUserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            });
        }

        // ── Delete user ────────────────────────────────────
        public async Task<(bool Success, string Message)>
            DeleteUserAsync(int userId, int requestingAdminId)
        {
            if (userId == requestingAdminId)
                return (false, "You cannot delete your own account.");

            var user = await _db.Users.FindAsync(userId);
            if (user == null)
                return (false, "User not found.");

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return (true, "User deleted.");
        }

        // ── Reset user password ────────────────────────────
        public async Task<(bool Success, string Message)>
            ResetPasswordAsync(int userId, string newPassword)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null)
                return (false, "User not found.");

            if (newPassword.Length < 6)
                return (false, "Password must be at least 6 characters.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _db.SaveChangesAsync();
            return (true, "Password reset successfully.");
        }

        // ── Get all courses (admin view) ───────────────────
        public async Task<List<CourseDto>> GetAllCoursesAsync()
        {
            return await _db.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                .Include(c => c.Modules)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    IsPublished = c.IsPublished,
                    CreatedAt = c.CreatedAt,
                    InstructorId = c.InstructorId,
                    InstructorName = c.Instructor.FullName,
                    EnrollmentCount = c.Enrollments.Count,
                    ModuleCount = c.Modules.Count
                })
                .ToListAsync();
        }
    }
}