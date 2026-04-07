namespace LearningHub.API.DTOs
{
    public class MaterialDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
        public int ModuleId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateMaterialDto
    {
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Video, Document, Link
        public int OrderIndex { get; set; }
    }

    public class UpdateMaterialDto
    {
        public string Title { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
    }
}