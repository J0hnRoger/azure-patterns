using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using AzElevator.AzureAIHandsOn.Services.LLM.Models;

namespace AzElevator.AzureAIHandsOn.Services.LLM;

public class LocalLLMService : ILLMService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _model;

    public string Name { get => "local"; }

    public LocalLLMService(IConfiguration config)
    {
        _baseUrl = config["LocalLLM:BaseUrl"] ?? "http://localhost:1234";
        _model = config["LocalLLM:Model"] ?? "model-identifier";

        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(_baseUrl);
        _httpClient.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<string> GetCompletion(string systemPrompt, string userPrompt)
    {
        var request = new LLMRequest(
            Model: _model,
            Messages: new[] {new LLMMessage("assistant", systemPrompt), new LLMMessage("user", userPrompt)},
            Temperature: 0.7,
            Stream: false
        );

        string serializedRequest = JsonConvert.SerializeObject(request,
            new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver(),});
        
        var response = await _httpClient.PostAsync(
            $"v1/chat/completions",
            new StringContent(serializedRequest, System.Text.Encoding.UTF8, "application/json")
        );

        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Local LLM request failed: {responseContent}");

        var localResponse = JsonConvert.DeserializeObject<LocalLLMResponse>(responseContent);
        
        // Mapping vers le format commun LLMResponse
        var llmResponse = new LLMResponse(
            Choices: localResponse?.Choices
                .Select(c => new Choice(
                    Message: new Message(
                        Role: c.Message.Role,
                        Content: c.Message.Content
                    )
                ))
                .ToArray() ?? Array.Empty<Choice>()
        );

        return llmResponse?.Choices?[0]?.Message?.Content ?? "No response generated.";
    }
}