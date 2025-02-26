using System.Text.Json.Serialization;

namespace AzElevator.AzureAIHandsOn.Services;

public class TokenUsage
{
    [JsonPropertyName("total_tokens")] public int TotalTokens { get; set; }
}