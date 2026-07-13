namespace ResumeAnalyzer.API.Interfaces;

public interface IResumeIndexingService
{
    Task IndexResumeAsync(string extractedText);
}
