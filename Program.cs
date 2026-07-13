using ResumeAnalyzer.API.Services;
using ResumeAnalyzer.API.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IResumeService, ResumeService>();
builder.Services.AddScoped<ITextChnukingService, TextChunkingService>();
builder.Services.AddScoped<IResumeParserService, ResumeParserService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();