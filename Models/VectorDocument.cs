namespace ResumeAnalyzer.API.Models
{
    public class VectorDocument
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string SectionName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public float[] EMbedding { get; set; } = Array.Empty<float>();
    }
}
