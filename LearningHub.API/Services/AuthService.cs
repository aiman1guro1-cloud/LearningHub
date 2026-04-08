using LearningHub.API.Data;
using LearningHub.API.DTOs;
using LearningHub.API.Helpers;
using LearningHub.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningHub.API.Services
{
    public class AuthService
    {
        private readonly AppDbContext _db;
        private readonly JwtHelper _jwt;

        public AuthService(AppDbContext db, JwtHelper jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
        {
            // Check if email already exists
            bool emailExists = await _db.Users.AnyAsync(u => u.Email == dto.Email);
            if (emailExists) return null;

            // Only allow Student or Instructor to self-register
            var allowedRoles = new[] { "Student", "Instructor" };
            var role = allowedRoles.Contains(dto.Role) ? dto.Role : "Student";

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = role
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return new AuthResponseDto
            {
                Token = _jwt.GenerateToken(user),
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                UserId = user.Id
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null) return null;

            bool validPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!validPassword) return null;

            return new AuthResponseDto
            {
                Token = _jwt.GenerateToken(user),
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                UserId = user.Id
            };
        }

        public async Task<(bool Success, string Message)> ChangePasswordAsync(
            int userId, ChangePasswordDto dto)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return (false, "User not found.");

            bool valid = BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash);
            if (!valid) return (false, "Current password is incorrect.");

            if (dto.NewPassword.Length < 6)
                return (false, "New password must be at least 6 characters.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _db.SaveChangesAsync();
            return (true, "Password changed successfully.");
        }
    }
}