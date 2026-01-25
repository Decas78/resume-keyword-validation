using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAntiforgery();

var app = builder.Build();

app.Urls.Add("http://0.0.0.0:5000");

app.UseAntiforgery();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Resume API is running.");

app.MapGet("/generate-token", (
    IAntiforgery antiforgery,
    HttpContext httpContext) =>
    new {antiforgery.GetAndStoreTokens(httpContext).RequestToken });


app.MapPost("/upload-file", async (
	IFormFile file,
	[FromForm(Name = "__RequestVerificationToken")] string token
	) =>
{
    var directory = $"{Environment.CurrentDirectory}/Files";
    
    if (!Directory.Exists(directory))
    {
        Directory.CreateDirectory(directory);
    }
    var fullFilePath = $"{directory}/{file.FileName}";

    if (!File.Exists(fullFilePath))
    {
        using var fileStream = File.Create(fullFilePath);
        await file.CopyToAsync(fileStream);
        return new {Success = true};
    }

    return new {Success = false};
});

app.Run();