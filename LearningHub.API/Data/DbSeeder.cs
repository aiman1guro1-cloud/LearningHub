using LearningHub.API.Models;

namespace LearningHub.API.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            // Only seed if no users exist
            if (context.Users.Any()) return;

            context.Users.Add(new User
            {
                FullName = "System Admin",
                Email = "admin@learninghub.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            });

            context.SaveChanges();
        }
    }
}