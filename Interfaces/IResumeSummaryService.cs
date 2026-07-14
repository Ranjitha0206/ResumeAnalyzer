namespace ResumeAnalyzer.API.Interfaces;

public interface IResumeSummaryService
{
    Task<string> GenerateSummaryAsync();
}
