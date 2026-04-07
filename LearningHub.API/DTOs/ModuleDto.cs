namespace LearningHub.API.DTOs
{
    public class ModuleDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
        public int CourseId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<MaterialDto> Materials { get; set; } = new();
    }

    public class CreateModuleDto
    {
        public string Title { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
    }

    public class UpdateModuleDto
    {
        public string Title { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
    }
}