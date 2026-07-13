using Microsoft.AspNetCore.Mvc;
using ResumeAnalyzer.API.Models;

namespace ResumeAnalyzer.API.Interfaces;


public interface ITextChnukingService
{
    //List<string> ChunkText(string text, int chunkSize = 500);

    List<ResumeSection> ChunkResume(string resumeText);
}

