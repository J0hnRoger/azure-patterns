namespace AzElevator.AzureAIHandsOn.Services.LLM.Models;

public record LocalLLMResponse(
    string Id,
    string Object,
    long Created,
    string Model,
    LocalLLMChoice[] Choices,
    LocalLLMUsage Usage,
    Dictionary<string, object> Stats,
    string SystemFingerprint
);

public record LocalLLMChoice(
    int Index,
    object Logprobs,
    string FinishReason,
    LocalLLMMessage Message
);

public record LocalLLMMessage(
    string Role,
    string Content
);

public record LocalLLMUsage(
    int PromptTokens,
    int CompletionTokens,
    int TotalTokens
); 