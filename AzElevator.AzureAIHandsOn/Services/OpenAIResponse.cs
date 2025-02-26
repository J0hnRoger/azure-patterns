using System.Text.Json.Serialization;

namespace AzElevator.AzureAIHandsOn.Services;

public class OpenAIResponse
{
    [JsonPropertyName("choices")] public Choice[] Choices { get; set; }
}