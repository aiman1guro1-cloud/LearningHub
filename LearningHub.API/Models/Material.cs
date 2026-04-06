namespace LearningHub.API.Models
{
    public class Material
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Video, Document, Link
        public string FileUrl { get; set; } = string.Empty;
        public int OrderIndex { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key
        public int ModuleId { get; set; }
        public Module Module { get; set; } = null!;

        // Navigation properties
        public ICollection<Progress> Progresses { get; set; } = new List<Progress>();
    }
}