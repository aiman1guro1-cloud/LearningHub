namespace LearningHub.API.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public int StudentId { get; set; }
        public User Student { get; set; } = null!;

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        // Navigation properties
        public ICollection<Progress> Progresses { get; set; } = new List<Progress>();
    }
}