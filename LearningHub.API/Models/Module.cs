namespace LearningHub.API.Models
{
    public class Module
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int OrderIndex { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        // Navigation properties
        public ICollection<Material> Materials { get; set; } = new List<Material>();
    }
}