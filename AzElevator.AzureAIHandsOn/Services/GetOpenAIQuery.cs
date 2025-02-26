using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using System.Text.Json;
using CSharpFunctionalExtensions;
using TiktokenSharp;

namespace AzElevator.AzureAIHandsOn.Services;

public class GetOpenAIQuery
{
    private readonly ILogger<GetOpenAIQuery> _logger;

    private readonly SearchClient _searchClient;
    private readonly HttpClient _httpClient;

    private readonly string _openAiEndpoint;
    private readonly string _openAiModel;

    public GetOpenAIQuery(IConfiguration config, ILogger<GetOpenAIQuery> logger)
    {
        _logger = logger;
        var searchEndpoint = config["AzureAI:SearchEndpoint"]!;
        var searchKey = config["AzureAI:SearchKey"]!;
        var indexName = config["AzureAI:IndexName"]!;

        _searchClient = new SearchClient(
            new Uri(searchEndpoint),
            indexName,
            new AzureKeyCredential(searchKey)
        );

        _openAiEndpoint = config["AzureAI:OpenAIEndpoint"]!;
        _openAiModel = config["AzureAI:OpenAIModel"]!;

        string openAiKey = config["AzureAI:OpenAIKey"]!;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("api-key", openAiKey);
    }

    public async Task<Result<string>> Execute(string query)
    {
        // Étape 1 : Recherche dans Azure AI Search
        var searchResults = await _searchClient.SearchAsync<SearchDocument>(
            query,
            new SearchOptions {Size = 3, QueryType = SearchQueryType.Simple, Select = {"content"}}
        );

        List<string> contexts = new();
        await foreach (var result in searchResults.Value.GetResultsAsync())
        {
            var content = result.Document["content"]?.ToString() ?? "";
            var cleanedContent = CleanText(content); // 🔥 Nettoyage du texte
            contexts.Add(cleanedContent);
        }

        // Étape 2 : Construction du prompt
        string contextText = string.Join("\n---\n", contexts);
        string prompt = $@"
            CONTEXTE :
            {contextText}

            QUESTION :
            {query}

            RÉPONSE :
        ";

        int tokenCount = await GetTokenCount(prompt);
        _logger.LogInformation("Nombre de tokens utilisés: {tokenCount}", tokenCount);

        // ✉️ Étape 3 : Envoi à OpenAI GPT-4
        var requestBody = new
        {
            model = _openAiModel,
            messages = new[]
            {
                new
                {
                    role = "system",
                    content =
                        "Tu es un expert Marketing qui m'aide à transformer ma base de connaissances en post virales pour 'hook' mes clients et leur vendre mes services de développement d'un MVP sur Azure" +
                    "- **Accroche percutante** : Une phrase choc qui capte l'attention dès le début.\n" +
                    "- **Problème clé** : Un problème spécifique auquel fait face le persona ciblé (exemple : 'scalabilité imprévisible' pour les CTO).\n"
                    +
                    "- **Solution innovante** : Mettre en avant une solution basée sur le Cloud Azure en expliquant ses avantages concrets.\n"
                    +
                    "- **Preuve sociale** : Inclure une statistique percutante ou un témoignage.\n" +
                    "- **Appel à l'action (CTA)** : Inviter à commenter, partager ou se connecter avec moi.\n" +
                    "Le post ne doit pas dépasser 2200 caractères."
                },
                
                new {role = "user", content = prompt}
            },
            temperature = 0.7
        };

        var response = await _httpClient.PostAsync(
            $"{_openAiEndpoint}/openai/deployments/{_openAiModel}/chat/completions?api-version=2023-05-15",
            new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json")
        );

        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            return Result.Failure<string>(responseContent);

        var jsonResponse = JsonSerializer.Deserialize<OpenAIResponse>(responseContent);

        return jsonResponse?.Choices?[0]?.Message?.Content ?? "Je n'ai pas trouvé de réponse pertinente.";
    }

    private async Task<int> GetTokenCount(string text)
    {
        var encoding = await TikToken.GetEncodingAsync("cl100k_base"); // Modèle de tokenisation pour GPT-4 et GPT-3.5

        var tokens = encoding.Encode(text);
        return tokens.Count;
    }

    public static string CleanText(string text)
    {
        text = text.Replace("\r", "");
        text = System.Text.RegularExpressions.Regex.Replace(text, @"\s{2,}",
            " ");
        text = System.Text.RegularExpressions.Regex.Replace(text, @"\n{2,}",
            "\n");
        text = text.Trim();
        return text;
    }
}