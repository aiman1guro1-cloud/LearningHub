namespace LearningHub.API.DTOs
{
    public class ProgressDto
    {
        public int Id { get; set; }
        public int EnrollmentId { get; set; }
        public int MaterialId { get; set; }
        public string MaterialTitle { get; set; } = string.Empty;
        public DateTime CompletedAt { get; set; }
    }

    public class MarkCompleteDto
    {
        public int CourseId { get; set; }
        public int MaterialId { get; set; }
    }

    public class CourseProgressDto
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public int TotalMaterials { get; set; }
        public int CompletedMaterials { get; set; }
        public double ProgressPercent { get; set; }
        public List<ProgressDto> CompletedItems { get; set; } = new();
    }
}