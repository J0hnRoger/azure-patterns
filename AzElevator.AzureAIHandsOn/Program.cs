using AzElevator.AzureAIHandsOn.Services;
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

app.MapPost("/ask", ([FromBody] QuestionDto question, [FromServices] GetOpenAIQuery handler) =>
{
    return Results.Ok(handler.Execute(question.Query));
});

app.Run();

public class QuestionDto
{
    public string Query { get; set; }
}
