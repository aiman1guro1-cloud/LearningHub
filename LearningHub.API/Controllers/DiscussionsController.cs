using LearningHub.API.DTOs;
using LearningHub.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearningHub.API.Controllers
{
    [ApiController]
    [Route("api/courses/{courseId}/discussions")]
    public class DiscussionsController : ControllerBase
    {
        private readonly DiscussionService _discussionService;

        public DiscussionsController(DiscussionService discussionService)
        {
            _discussionService = discussionService;
        }

        // GET api/courses/{courseId}/discussions
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetPosts(int courseId)
        {
            var posts = await _discussionService.GetPostsByCourseAsync(courseId);
            return Ok(posts);
        }

        // GET api/courses/{courseId}/discussions/{postId}
        [Authorize]
        [HttpGet("{postId}")]
        public async Task<IActionResult> GetPost(int courseId, int postId)
        {
            var post = await _discussionService.GetPostDetailAsync(postId);
            if (post == null) return NotFound(new { message = "Post not found." });
            return Ok(post);
        }

        // POST api/courses/{courseId}/discussions
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePost(int courseId, [FromBody] CreateDiscussionPostDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Content))
                return BadRequest(new { message = "Title and content are required." });

            int userId = GetUserId();
            string role = GetRole();

            var (success, message, data) =
                await _discussionService.CreatePostAsync(courseId, userId, role, dto);

            if (!success) return BadRequest(new { message });
            return Ok(data);
        }

        // PUT api/courses/{courseId}/discussions/{postId}
        [Authorize]
        [HttpPut("{postId}")]
        public async Task<IActionResult> UpdatePost(
            int courseId, int postId, [FromBody] UpdateDiscussionPostDto dto)
        {
            int userId = GetUserId();
            string role = GetRole();

            var (success, message, data) =
                await _discussionService.UpdatePostAsync(postId, userId, role, dto);

            if (!success) return BadRequest(new { message });
            return Ok(data);
        }

        // DELETE api/courses/{courseId}/discussions/{postId}
        [Authorize]
        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost(int courseId, int postId)
        {
            int userId = GetUserId();
            string role = GetRole();

            var (success, message) =
                await _discussionService.DeletePostAsync(postId, userId, role);

            if (!success) return BadRequest(new { message });
            return NoContent();
        }

        // POST api/courses/{courseId}/discussions/{postId}/comments
        [Authorize]
        [HttpPost("{postId}/comments")]
        public async Task<IActionResult> AddComment(
            int courseId, int postId, [FromBody] CreateCommentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Content))
                return BadRequest(new { message = "Comment cannot be empty." });

            int userId = GetUserId();
            string role = GetRole();

            var (success, message, data) =
                await _discussionService.AddCommentAsync(postId, userId, role, dto);

            if (!success) return BadRequest(new { message });
            return Ok(data);
        }

        // DELETE api/courses/{courseId}/discussions/{postId}/comments/{commentId}
        [Authorize]
        [HttpDelete("{postId}/comments/{commentId}")]
        public async Task<IActionResult> DeleteComment(
            int courseId, int postId, int commentId)
        {
            int userId = GetUserId();
            string role = GetRole();

            var (success, message) =
                await _discussionService.DeleteCommentAsync(commentId, userId, role);

            if (!success) return BadRequest(new { message });
            return NoContent();
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private string GetRole() => User.FindFirstValue(ClaimTypes.Role)!;
    }
}