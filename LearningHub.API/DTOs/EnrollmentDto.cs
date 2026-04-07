namespace LearningHub.API.DTOs
{
    public class EnrollmentDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public DateTime EnrolledAt { get; set; }
        public int CompletedMaterials { get; set; }
        public int TotalMaterials { get; set; }
        public double ProgressPercent { get; set; }
    }

    public class EnrollRequestDto
    {
        public int CourseId { get; set; }
    }
}