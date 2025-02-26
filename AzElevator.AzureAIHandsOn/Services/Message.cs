using System.Text.Json.Serialization;

namespace AzElevator.AzureAIHandsOn.Services;

public class Message
{
    [JsonPropertyName("content")] public string Content { get; set; }
}