using Microsoft.AspNetCore.Http;

namespace ResumeAnalyzer.API.Interfaces;

public interface IResumeService
{
    Task<string> UploadResumeAsync(IFormFile file);

    string ExtractText(string filePath);
}
