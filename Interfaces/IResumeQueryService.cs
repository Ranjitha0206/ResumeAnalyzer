namespace ResumeAnalyzer.API.Interfaces;

public interface IResumeQueryService
{
    Task<string> AskAsync(string question);
}
