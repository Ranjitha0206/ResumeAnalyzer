using ResumeAnalyzer.API.Models;

namespace ResumeAnalyzer.API.Interfaces;

public interface IResumeParserService
{
    List<ResumeSection> ParseResume(string resumeText);
}
