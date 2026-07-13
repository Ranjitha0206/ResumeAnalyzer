using ResumeAnalyzer.API.Interfaces;
using System.Text;
using UglyToad.PdfPig;
namespace ResumeAnalyzer.API.Services;

public class ResumeService : IResumeService
{
    private readonly IWebHostEnvironment _environment;  

    public ResumeService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> UploadResumeAsync(IFormFile file)
    {
        var uploadsFolder = Path.Combine(_environment.ContentRootPath, "Uploads");

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";

        var filePath = Path.Combine(uploadsFolder, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);

        await file.CopyToAsync(stream);

        return filePath;
    }

    public string ExtractText(string filePath)
    {
        StringBuilder text = new StringBuilder();

        using (PdfDocument document = PdfDocument.Open(filePath))
        {
            foreach(var page in document.GetPages())
            {
                //text.AppendLine(page.Text);
                text.AppendLine(page.Text);
            }
        }
        return text.ToString();
    }
}
