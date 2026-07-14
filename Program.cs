using ResumeAnalyzer.API.Services;
using ResumeAnalyzer.API.Interfaces;
using ResumeAnalyzer.API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IResumeService, ResumeService>();
builder.Services.AddScoped<ITextChnukingService, TextChunkingService>();
builder.Services.AddScoped<IResumeParserService, ResumeParserService>();
builder.Services.AddScoped<IEmbeddingService, AIEmbeddingService>();
builder.Services.AddSingleton<IVectorStore, InMemoryVectorStore>();
builder.Services.AddScoped<IResumeIndexingService, ResumeIndexingService>();
builder.Services.AddScoped<IChatService, GeminiChatService>();
builder.Services.AddScoped<IResumeQueryService, ResumeQueryService>();
builder.Services.AddScoped<IResumeSummaryService, ResumeSummaryService>();
builder.Services.AddScoped<IResumeSkillsService, ResumeSkillsService>();
builder.Services.Configure<GeminiSettings>(builder.Configuration.GetSection("Gemini"));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();