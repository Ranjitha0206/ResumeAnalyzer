namespace ResumeAnalyzer.API.Interfaces;

public interface IChatService
{
    Task<string> AskAsync(String question, IEnumerable<string> context);
}
