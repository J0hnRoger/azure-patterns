using System.Text.Json.Serialization;

namespace AzElevator.AzureAIHandsOn.Services;

public class TokenUsageResponse
{
    [JsonPropertyName("usage")] public TokenUsage Usage { get; set; }
}