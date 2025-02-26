namespace AzElevator.AzureAIHandsOn.Services.LLM;

public interface ILLMService
{
    public string Name { get; }
    Task<string> GetCompletion(string systemPrompt, string userPrompt);
}

public record LLMMessage(string Role, string Content);
public record LLMRequest(string Model, LLMMessage[] Messages, double Temperature, int? MaxTokens = null, bool Stream = false);
public record LLMResponse(Choice[] Choices);
public record Choice(Message Message);
public record Message(string Role, string Content); 