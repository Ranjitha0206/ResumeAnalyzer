namespace ResumeAnalyzer.API.Interfaces;

public interface IEmbeddingService
{
    Task<IReadOnlyList<double>> GenerateEmbeddingAsync(string text);
}
