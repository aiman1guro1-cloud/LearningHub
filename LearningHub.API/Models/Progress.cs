namespace LearningHub.API.Models
{
    public class Progress
    {
        public int Id { get; set; }
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public int EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; } = null!;

        public int MaterialId { get; set; }
        public Material Material { get; set; } = null!;
    }
}