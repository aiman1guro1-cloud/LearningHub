namespace LearningHub.API.Models
{
    public class DiscussionPost
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Navigation properties
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}