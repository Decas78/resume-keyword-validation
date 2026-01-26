using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAntiforgery();
builder.Services.AddHttpClient();

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


app.MapPost("/upload-resume", async (
	IFormFile file,
	[FromForm(Name = "__RequestVerificationToken")] string token,
    IHttpClientFactory httpClientFactory,
    CancellationToken ct) =>
{
    if (file == null || file.Length == 0)
    {
        return Results.BadRequest("No file uploaded.");
    }
    var pythonUrl = "http://processor:8000/process-resume";
    using var client = httpClientFactory.CreateClient();
    using var formContent = new MultipartFormDataContent();

    var fileContent = new StreamContent(file.OpenReadStream());

    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
    formContent.Add(fileContent, "file", file.FileName);

    try
    {
        var response = await client.PostAsync(pythonUrl, formContent, ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            return Results.StatusCode((int)response.StatusCode);
        }

        // Read response from Python service
        var pythonResult = await response.Content.ReadAsStringAsync(ct);
        return Results.Ok(pythonResult);
    }
    catch (HttpRequestException ex)
    {
        return Results.Problem("Failed to reach Python service: " + ex.Message);
    }
});

app.Run();