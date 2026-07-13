namespace ResumeAnalyzer.API.Interfaces;

public interface IEmbeddingService
{
    Task<float[]> GenerateEmbeddingAsync(string text);
}
