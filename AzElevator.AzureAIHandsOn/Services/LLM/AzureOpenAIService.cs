using System.Text.Json;

namespace AzElevator.AzureAIHandsOn.Services.LLM;

public class AzureOpenAIService : ILLMService
{
    private readonly HttpClient _httpClient;
    private readonly string _endpoint;
    private readonly string _model;

    public string Name { get => "azure"; }
    
    public AzureOpenAIService(IConfiguration config)
    {
        _endpoint = config["AzureAI:OpenAIEndpoint"]!;
        _model = config["AzureAI:OpenAIModel"]!;
        string openAiKey = config["AzureAI:OpenAIKey"]!;
        
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("api-key", openAiKey);
    }

    public async Task<string> GetCompletion(string systemPrompt, string userPrompt)
    {
        var request = new LLMRequest(
            Model: _model,
            Messages: new[]
            {
                new LLMMessage("system", systemPrompt),
                new LLMMessage("user", userPrompt)
            },
            Temperature: 0.7
        );

        var response = await _httpClient.PostAsync(
            $"{_endpoint}/openai/deployments/{_model}/chat/completions?api-version=2023-05-15",
            new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json")
        );

        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Azure OpenAI request failed: {responseContent}");

        var jsonResponse = JsonSerializer.Deserialize<LLMResponse>(responseContent);
        return jsonResponse?.Choices?[0]?.Message?.Content ?? "No response generated.";
    }
} 