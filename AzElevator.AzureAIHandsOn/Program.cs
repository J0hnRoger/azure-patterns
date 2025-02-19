using AzElevator.AzureAIHandsOn.Services;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<GetOpenAIQuery>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/ask", async ([FromBody] QuestionDto question, [FromServices] GetOpenAIQuery handler) =>
{
    Result<string> response = await handler.Execute(question.Query);
    if (response.IsFailure)
        return Results.Problem(response.Error);
    
    return Results.Ok(response.Value);
});

app.Run();

public class QuestionDto
{
    public string Query { get; set; }
}
