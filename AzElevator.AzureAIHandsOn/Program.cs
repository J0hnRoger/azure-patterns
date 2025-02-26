using AzElevator.AzureAIHandsOn.Services;
using AzElevator.AzureAIHandsOn.Services.LLM;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<GetOpenAIQuery>();
builder.Services.AddScoped<GetLinkedinInboundMarketingPostQuery>();

// Enregistrement des deux services avec leurs clés
builder.Services.AddSingleton<ILLMService, AzureOpenAIService>();
builder.Services.AddSingleton<ILLMService, LocalLLMService>();

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

app.MapPost("/marketer", async ([FromBody] QuestionDto question,
    [FromQuery] string llm,
    [FromServices] IEnumerable<ILLMService> llmProvider 
) =>
{
    
    // Sélection du service en fonction du paramètre
    var selectedService = llm switch
    {
        LLMServiceKeys.Local => llmProvider.First(model => model.Name == llm),
        _ => llmProvider.First(model => model.Name == LLMServiceKeys.Azure)
    };

    var handler = new GetLinkedinInboundMarketingPostQuery(
        app.Configuration,
        app.Services.GetRequiredService<ILogger<GetLinkedinInboundMarketingPostQuery>>(),
        selectedService);

    Result<GetOpenAIQueryResponse> response = await handler.Execute(question.Query);
    if (response.IsFailure)
        return Results.Problem(response.Error);
    
    return Results.Ok(response.Value);
});
app.Run();

public class QuestionDto
{
    public string Query { get; set; }
}
