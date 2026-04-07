using LearningHub.API.Data;
using LearningHub.API.DTOs;
using LearningHub.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningHub.API.Services
{
    public class DiscussionService
    {
        private readonly AppDbContext _db;

        public DiscussionService(AppDbContext db)
        {
            _db = db;
        }

        // ── Get all posts for a course ─────────────────────
        public async Task<List<DiscussionPostDto>> GetPostsByCourseAsync(int courseId)
        {
            return await _db.DiscussionPosts
                .Where(p => p.CourseId == courseId)
                .Include(p => p.User)
                .Include(p => p.Comments)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new DiscussionPostDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    CourseId = p.CourseId,
                    UserId = p.UserId,
                    AuthorName = p.User.FullName,
                    AuthorRole = p.User.Role,
                    CreatedAt = p.CreatedAt,
                    CommentCount = p.Comments.Count,
                    Comments = new List<CommentDto>()
                })
                .ToListAsync();
        }

        // ── Get single post with all comments ─────────────
        public async Task<DiscussionPostDto?> GetPostDetailAsync(int postId)
        {
            var post = await _db.DiscussionPosts
                .Include(p => p.User)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null) return null;

            return new DiscussionPostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CourseId = post.CourseId,
                UserId = post.UserId,
                AuthorName = post.User.FullName,
                AuthorRole = post.User.Role,
                CreatedAt = post.CreatedAt,
                CommentCount = post.Comments.Count,
                Comments = post.Comments
                    .OrderBy(c => c.CreatedAt)
                    .Select(c => new CommentDto
                    {
                        Id = c.Id,
                        Content = c.Content,
                        PostId = c.PostId,
                        UserId = c.UserId,
                        AuthorName = c.User.FullName,
                        AuthorRole = c.User.Role,
                        CreatedAt = c.CreatedAt
                    }).ToList()
            };
        }

        // ── Create a post ──────────────────────────────────
        public async Task<(bool Success, string Message, DiscussionPostDto? Data)>
            CreatePostAsync(int courseId, int userId, string role, CreateDiscussionPostDto dto)
        {
            // Students must be enrolled
            if (role == "Student")
            {
                bool enrolled = await _db.Enrollments
                    .AnyAsync(e => e.StudentId == userId && e.CourseId == courseId);
                if (!enrolled)
                    return (false, "You must be enrolled to post in this course.", null);
            }

            // Instructors must own the course
            if (role == "Instructor")
            {
                bool ownsCourse = await _db.Courses
                    .AnyAsync(c => c.Id == courseId && c.InstructorId == userId);
                if (!ownsCourse)
                    return (false, "You do not have access to this course.", null);
            }

            var post = new DiscussionPost
            {
                Title = dto.Title,
                Content = dto.Content,
                CourseId = courseId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _db.DiscussionPosts.Add(post);
            await _db.SaveChangesAsync();

            var user = await _db.Users.FindAsync(userId);

            return (true, "Post created.", new DiscussionPostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CourseId = post.CourseId,
                UserId = post.UserId,
                AuthorName = user?.FullName ?? "",
                AuthorRole = role,
                CreatedAt = post.CreatedAt,
                CommentCount = 0,
                Comments = new List<CommentDto>()
            });
        }

        // ── Update a post ──────────────────────────────────
        public async Task<(bool Success, string Message, DiscussionPostDto? Data)>
            UpdatePostAsync(int postId, int userId, string role, UpdateDiscussionPostDto dto)
        {
            var post = await _db.DiscussionPosts
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
                return (false, "Post not found.", null);

            // Only the author or Admin can edit
            if (post.UserId != userId && role != "Admin")
                return (false, "You can only edit your own posts.", null);

            post.Title = dto.Title;
            post.Content = dto.Content;
            await _db.SaveChangesAsync();

            return (true, "Post updated.", new DiscussionPostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CourseId = post.CourseId,
                UserId = post.UserId,
                AuthorName = post.User.FullName,
                AuthorRole = post.User.Role,
                CreatedAt = post.CreatedAt
            });
        }

        // ── Delete a post ──────────────────────────────────
        public async Task<(bool Success, string Message)>
            DeletePostAsync(int postId, int userId, string role)
        {
            var post = await _db.DiscussionPosts.FindAsync(postId);
            if (post == null) return (false, "Post not found.");

            if (post.UserId != userId && role != "Admin" && role != "Instructor")
                return (false, "You cannot delete this post.");

            _db.DiscussionPosts.Remove(post);
            await _db.SaveChangesAsync();
            return (true, "Post deleted.");
        }

        // ── Add a comment ──────────────────────────────────
        public async Task<(bool Success, string Message, CommentDto? Data)>
            AddCommentAsync(int postId, int userId, string role, CreateCommentDto dto)
        {
            var post = await _db.DiscussionPosts
                .Include(p => p.Course)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null) return (false, "Post not found.", null);

            // Students must be enrolled in the course
            if (role == "Student")
            {
                bool enrolled = await _db.Enrollments
                    .AnyAsync(e => e.StudentId == userId && e.CourseId == post.CourseId);
                if (!enrolled)
                    return (false, "You must be enrolled to comment.", null);
            }

            var comment = new Comment
            {
                Content = dto.Content,
                PostId = postId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();

            var user = await _db.Users.FindAsync(userId);

            return (true, "Comment added.", new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                PostId = comment.PostId,
                UserId = comment.UserId,
                AuthorName = user?.FullName ?? "",
                AuthorRole = role,
                CreatedAt = comment.CreatedAt
            });
        }

        // ── Delete a comment ───────────────────────────────
        public async Task<(bool Success, string Message)>
            DeleteCommentAsync(int commentId, int userId, string role)
        {
            var comment = await _db.Comments.FindAsync(commentId);
            if (comment == null) return (false, "Comment not found.");

            if (comment.UserId != userId && role != "Admin" && role != "Instructor")
                return (false, "You cannot delete this comment.");

            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync();
            return (true, "Comment deleted.");
        }
    }
}