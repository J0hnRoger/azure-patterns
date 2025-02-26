using System.Text.Json.Serialization;

namespace AzElevator.AzureAIHandsOn.Services;

public class Choice
{
    [JsonPropertyName("message")] public Message Message { get; set; }
}