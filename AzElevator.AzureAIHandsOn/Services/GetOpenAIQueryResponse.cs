namespace AzElevator.AzureAIHandsOn.Services;

public record GetOpenAIQueryResponse(string Answer, int TokenCount, List<string> Sources);