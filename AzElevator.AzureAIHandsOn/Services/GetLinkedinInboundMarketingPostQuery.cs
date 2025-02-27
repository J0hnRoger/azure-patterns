using System.Text.Json;
using AzElevator.AzureAIHandsOn.Services.LLM;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using CSharpFunctionalExtensions;
using TiktokenSharp;

namespace AzElevator.AzureAIHandsOn.Services;

public class GetLinkedinInboundMarketingPostQuery
{
    private readonly SearchClient _searchClient;
    private readonly ILogger<GetLinkedinInboundMarketingPostQuery> _logger;
    private readonly ILLMService _llmService;
    readonly List<string> _marketingReferenceDatas = [];

    public GetLinkedinInboundMarketingPostQuery(
        IConfiguration config,
        ILogger<GetLinkedinInboundMarketingPostQuery> logger,
        ILLMService llmService)
    {
        _logger = logger;
        _llmService = llmService;
        
        var searchEndpoint = config["AzureAI:SearchEndpoint"]!;
        var searchKey = config["AzureAI:SearchKey"]!;
        var indexName = config["AzureAI:IndexName"]!;

        _searchClient = new SearchClient(
            new Uri(searchEndpoint),
            indexName,
            new AzureKeyCredential(searchKey)
        );

        SetReferenceDatas().Wait();
    }

    private async Task SetReferenceDatas()
    {
        var searchResults = await _searchClient.SearchAsync<SearchDocument>(
            "Copywriting et Smart Brevity",
            new SearchOptions {Size = 3, QueryType = SearchQueryType.Simple, Select = {"content"}}
        );
        
        await foreach (var result in searchResults.Value.GetResultsAsync())
        {
            var content = result.Document["content"]?.ToString() ?? "";
            var cleanedContent = CleanText(content); // 🔥 Nettoyage du texte
            
            _marketingReferenceDatas.Add(cleanedContent);
        }
    }

    public async Task<Result<GetOpenAIQueryResponse>> Execute(string query)
    {
        // Étape 1 : Recherche dans Azure AI Search
        var searchResults = await _searchClient.SearchAsync<SearchDocument>(
            query,
            new SearchOptions { Size = 5, QueryType = SearchQueryType.Simple, Select = {"content"}}
        );

        List<string> contexts = new();
        await foreach (var result in searchResults.Value.GetResultsAsync())
        {
            var content = result.Document["content"]?.ToString() ?? "";
            var cleanedContent = CleanText(content); // 🔥 Nettoyage du texte
            contexts.Add(cleanedContent);
        }

        // Étape 2 : Construction du prompt
        string referenceDataContext = string.Join("\n---\n utilises un formats de copywriting adapté et les principes suivants: ", _marketingReferenceDatas);
        string format =
            "Réponds toujours avec le type de structure de posts que tu as utilisé. Ajoutes systématiquement tes sources à la fin de la réponse";
        string contextText = string.Join("\n---\n", contexts);
        string prompt = $@"
            CONTEXTE :
            {referenceDataContext}
            {contextText}

            QUESTION :
            {format}
            {query}

            RÉPONSE :
        ";

        int tokenCount = await GetTokenCount(prompt);
        _logger.LogInformation("Nombre de tokens utilisés: {tokenCount}", tokenCount);

        try
        {
            string systemPrompt = 
                        "Tu es un expert Marketing qui m'aide à transformer ma base de connaissances de CTO/Lead Tech .Net Azure en post virales pour 'hook' mes clients et leur vendre mes services de développement d'un MVP sur Azure" +
                    "- **Accroche percutante** : Une phrase choc qui capte l'attention dès le début.\n" +
                    "- **Problème clé** : Un problème spécifique auquel fait face le persona ciblé (exemple : 'scalabilité imprévisible' pour les CTO).\n"
                    +
                    "- **Solution innovante** : Mettre en avant une solution basée sur le Cloud Azure en expliquant ses avantages concrets.\n"
                    +
                    "- **Preuve sociale** : Inclure une statistique percutante ou un témoignage.\n" +
                    "- **Appel à l'action (CTA)** : Inviter à commenter, partager ou se connecter avec moi.\n" +
                    "Le post ne doit pas dépasser 2200 caractères."
;
            string answer = await _llmService.GetCompletion(systemPrompt, prompt);
            return new GetOpenAIQueryResponse(answer, tokenCount, []);
        }
        catch (Exception ex)
        {
            return Result.Failure<GetOpenAIQueryResponse>($"Error calling LLM service: {ex.Message}");
        }
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