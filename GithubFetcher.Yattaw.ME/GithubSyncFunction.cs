using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
namespace GithubFetcher.Yattaw.ME;

public class GithubSyncFunction
{
    private static readonly HttpClient HttpClient = new();
    private static readonly string GithubApiUrl = "https://api.github.com/users/{0}/repos?per_page=100";

    public async Task<string> Handler(object input, ILambdaContext context)
    {
        var githubUsername = Environment.GetEnvironmentVariable("GITHUB_USERNAME");
        var tableName = Environment.GetEnvironmentVariable("DYNAMODB_TABLE_NAME");
        if (string.IsNullOrEmpty(githubUsername))
        {
            throw new Exception("GitHub username is not configured.");
        }

        var dynamoDbClient = new AmazonDynamoDBClient();
        var table = Table.LoadTable(dynamoDbClient, tableName);

        HttpClient.DefaultRequestHeaders.Add("User-Agent", "GithubSyncFunction");
        var response = await HttpClient.GetAsync(string.Format(GithubApiUrl, githubUsername));
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var repositories = JsonDocument.Parse(content).RootElement.EnumerateArray();

        foreach (var repo in repositories)
        {
            if (repo.GetProperty("fork").GetBoolean()) continue;

            var repoName = repo.GetProperty("name").GetString();
            var document = new Document
            {
                ["REPO#repoName"] = repoName,   // required partition key
                ["Description"] = repo.GetProperty("description").GetString(),
                ["Language"] = repo.GetProperty("language").GetString(),
                ["HtmlUrl"] = repo.GetProperty("html_url").GetString(),
                ["PushedAt"] = repo.GetProperty("pushed_at").GetDateTime().ToString("o")
            };

            await table.PutItemAsync(document);
        }

        return "GitHub sync completed.";
    }
}